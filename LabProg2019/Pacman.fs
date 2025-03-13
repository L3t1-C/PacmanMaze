(*
* LabProg2019 - Progetto di Programmazione a.a. 2019-20
* Pacman.fs: Pacman
*(C) 882713 Cantoni Letizia Gruppo:LabProg2019B_28  @ Universita' Ca' Foscari di Venezia
*)

module LabProg2019.Pacman

open System
open External
open Gfx
open Engine
open Maze

Engine.Hide_FPS <- true

System.Console.Clear()
let mutable W,H = 50,25
let mutable engine = new engine (W, H)


let rec rnd_px () =
    let col = rnd_color ()
    if col = Color.Black||col = Color.Yellow then rnd_px ()
    else pixel.filledGhost col
let fpx = if rnd_bool () then Some (rnd_px ()) else None

type CharInfo with
    static member wall = pixel.create (Config.WallForPacman, Color.DarkBlue)
    static member internal path = pixel.filled Color.Black
    member this.isWall = this = pixel.wall

[< NoEquality; NoComparison >]
type state = {
    player : sprite
    blockmaze : sprite
    finish : sprite
    enemies : (sprite * int * int)[]
    coins : (sprite * int * int)[]
    mutable score:int
    path_color: Color
}

let PathColor (st:state) color char z =
    let pixpath = pixel.create(char, color)
    ignore <| engine.create_and_register_sprite (image.rectangle (1, 1, pixpath), int (st.player.x), int (st.player.y), z)


//this function is only for the coins and the enemies, to put them in the maze if a cell is a passage 
let rec posrand()=
    let tryx=rnd_int 1 W-1
    let tryy=rnd_int 1 H-1  
    if(MazeGenerate.Grid.[int (tryx), int (tryy)] = Passage) then
        tryx,tryy
    else
        posrand() 

let main(widthchoose,heightchoose) =

    W <- widthchoose+10
    H <- heightchoose+10

    engine <- new engine (W, H+12)
    
    MazeGenerate <- generate(initializationMaze W H)

    let printmaze (maze: Maze): pixel[] = 
        let pixelarray = Array.zeroCreate ((maze.Height)*(maze.Width))    
        maze.Grid |> Array2D.iteri 
            (fun y x cell ->
                let c = 
                    match cell with
                    | Blocked -> pixel.wall
                    | Passage -> pixel.path
                if x<>W || y<>H then                   
                    let pos = x*W+y //i find the position of the coordinate in the array
                    pixelarray.[pos] <- c                   
            )
        pixelarray

    let blockmaze= engine.create_and_register_sprite (new image (W,H,(printmaze MazeGenerate)), 0, 0, 0)
   
//here i create the player sprite and the finish sprite  
    let player = engine.create_and_register_sprite (image.rectangle (1, 1, pixel.FilledPacman Color.Yellow,pixel.FilledPacman Color.Yellow),1, 1, 2)
    let finish = engine.create_and_register_sprite (image.rectangle (1, 1, pixel.filled Color.DarkRed,pixel.filled Color.DarkRed), W-3, H-2, 2) 
//here i create the enemies sprite list     
    let Enemies = [|
        for i = 1 to 5 do
        let px,py=posrand()
        let spr = engine.create_and_register_sprite (image.circle (0, rnd_px (),?filled_px = fpx),px,py, 3)               
        let d () = 1
        yield spr, d (), d ()
        |]  
//here i create the coins sprite list            
    let Coins = [|
            for i = 1 to 20 do
            let px,py=posrand()
            let spr = engine.create_and_register_sprite (image.circle (0, pixel.fillball Color.Yellow ,pixel.fillball Color.Yellow),px,py,1)               
            let d () = 1
            yield spr, d (), d ()
            |]
//here i create the sprite to see the score of the player 
    let Scorepanel = engine.create_and_register_sprite (image.rectangle (10, 3, pixel.filled Color.White, pixel.filled Color.White),1,H,4)  
//here i create the sprite to see the time i have in the drill mode    
    let dirllpanel = engine.create_and_register_sprite (image.rectangle (10, 3, pixel.filled Color.White, pixel.filled Color.White),W-11,H,4)
//here i create the sprite to see the time i have before i can start again the drill mode     
    let coolpanel = engine.create_and_register_sprite (image.rectangle (14, 3, pixel.filled Color.White, pixel.filled Color.White),W/3,H,4)
    //this function manage the movement of the enemies, the movement of the player and the movement in the drill mode
    let my_update (key : ConsoleKeyInfo option) (screen : wronly_raster) (inf : info) (st : state) =
        //this function control if the player can move in some direction
        let isWall (x,y) =
            if (MazeGenerate.Grid.[int (st.player.x + x), int (st.player.y + y)] = Passage) && ((int (st.player.x + x)< W-1 && int (st.player.y + y) < H-1) && (int (st.player.x + x) >0 && int (st.player.y + y)>0))  then  
                st.player.move_by (x,y)
            else st.player.move_by (0.,0.)
        //this function control that the player (in the drill mode) remains in the maze 
        let moveEV (x,y)=
            if ((int (st.player.x + x)< W-1 && int (st.player.y + y) < H-1) && (int (st.player.x + x) >0 && int (st.player.y + y)>0))  then  
                PathColor st st.path_color Config.filled_pixel_char 2 //this show where the player create a passage (destroy a wall) 
                st.player.move_by (x,y)

            else st.player.move_by (0.,0.)


        match key with
        | None ->st.player.move_by (0., 0.) 
        | Some key ->   
            match key.KeyChar with 
            |' ' ->  if (inf.timer - Config.trapano_attivato > 20.) then //this is for the cooldown, if it isn't finish you cannot activate the drill mode
                        Config.trapano_attivato <- inf.timer 
                        Config.boool <- 1 // this activate the drill mode
                     
                     st.player.move_by ( 0., 0.)

            |'w'|'W' -> if Config.boool = 0 then isWall(0.,-1.) 
                         else moveEV( 0., -1.) ;  MazeGenerate.Grid.[ int st.player.x, int st.player.y] <- Passage//turn the wall into a passage
                         

            |'s'|'S' -> if Config.boool = 0 then isWall(0., 1.) 
                         else moveEV( 0., 1.) ;  MazeGenerate.Grid.[ int st.player.x, int st.player.y] <- Passage
                        

            |'a'|'A' -> if Config.boool = 0 then isWall(-1., 0.) 
                         else moveEV( -1., 0.) ;  MazeGenerate.Grid.[ int st.player.x, int st.player.y] <- Passage
                         

            |'d'|'D' -> if Config.boool = 0 then isWall(1., 0.) 
                         else moveEV ( 1., 0.) ;  MazeGenerate.Grid.[ int st.player.x, int st.player.y] <- Passage
            
            |'q'|'Q'-> exit 0                  

            | _ -> st.player.move_by ( 0., 0.)
        

        if(inf.timer - Config.trapano_attivato > 5.) then Config.boool<-0 //stop the drill mode
        
        //this shows the time that you have in the drill mode
        dirllpanel.draw_text(sprintf"DRILL:%d" 5  ,1, 1, Color.Black, Color.White) 
        let mutable a = (int inf.timer - int Config.trapano_attivato)
        if (inf.timer - Config.trapano_attivato < 5.1 ) then 
            let mutable a= 5 - a //to make a countdown
            dirllpanel.draw_text(sprintf"DRILL:%d"a  ,1, 1, Color.Black, Color.White)
        //this shows the time you have before you can start again the drill mode     
        coolpanel.draw_text(sprintf"COOLDOWN:%d"15,1, 1, Color.Black, Color.White)
        let mutable b = (int inf.timer - int Config.trapano_attivato)
        if (inf.timer - Config.trapano_attivato < 20.1 && inf.timer - Config.trapano_attivato > 5. ) then 
            let mutable b= 20 - b 
            coolpanel.draw_text("______________",1, 1, Color.White, Color.White)
            coolpanel.draw_text(sprintf"COOLDOWN:%d"(b),1, 1, Color.Black, Color.White)

//here i manage the random movement of the sprites       
        let enemies = [|          
            for spr, dx, dy in st.enemies do 
                let isPassage (x:float,y:float) =
                    if MazeGenerate.Grid.[int x , int y] = Passage then true else false
    //i use a lots of controls to put the enemies into the passages in the maze
                let dx = if isPassage (spr.x+1., spr.y+0.)&& dx=1 && dy=0 then 1 
                             else if isPassage (spr.x-1., spr.y+0.) && dx = -1 && dy=0 then -1 
                             else if (not (isPassage (spr.x+1., spr.y+0.))&& dx=1 && dy=0) then (rand.Next 2 )* -1
                             else if (not (isPassage (spr.x-1., spr.y+0.))&& dx = -1 && dy=0) then rand.Next 2 
                             else if (dy<>0) then 0
                             else if ((dx=0&&dy=0)&&isPassage (spr.x+1., spr.y+0.))then 1
                             else if ((dx=0&&dy=0)&&isPassage (spr.x-1., spr.y+0.))then -1
                             else 0 
                         

                let dy = if isPassage (spr.x+0., spr.y+1.)&& dx=0 && dy=1 then 1 
                             else if isPassage (spr.x-0., spr.y-1.) && dx =0 && dy= -1 then -1 
                             else if (not (isPassage (spr.x+0., spr.y+1.))&& dx=0 && dy= 1) then (rand.Next 2 )* -1
                             else if (not (isPassage (spr.x-0., spr.y-1.))&& dx =0 && dy= -1) then rand.Next 2  
                             else if (dx<>0)then 0
                             else if ((dx=0&&dy=0)&&isPassage (spr.x+0., spr.y+1.))then 1
                             else if ((dx=0&&dy=0)&&isPassage (spr.x+0., spr.y-1.))then -1
                             else 0
                    
                //i move the enemies only if the frame count is divisible for 3 because, if not, they are too speed
                if (inf.frame_cnt%3 = 0) then
                    spr.move_by (dx,dy)
                
                yield spr, dx, dy
                
            |] 
        
        //this function enables you to achive the coins and increase your score
        for spr, dx, dy in st.coins do             
            if st.player.x = spr.x && st.player.y = spr.y then 
                   let  mutable score1= st.score + 1
                   st.score<-score1
                   spr.clear
                   Scorepanel.draw_text(sprintf"SCORE:%d" score1 ,1, 1, Color.Black, Color.White)
                   spr.x<-0.
                   spr.y<-0.

               else
                   Scorepanel.draw_text(sprintf"SCORE:%d" st.score ,1, 1, Color.Black, Color.White)
        
        Config.punteggio <- st.score 

        //if you touch an enemy you'll die
        for spr, dx, dy in st.enemies do 
            if st.player.x = spr.x && st.player.y = spr.y then 
                FinishScreen.FinishResult(false)

        //if you arrive at the finish sprite you win
        if st.player.x = st.finish.x && st.player.y = st.finish.y then 
            Config.finishpac<-1; FinishScreen.FinishResult(true)            

        { 
            player = player
            blockmaze = blockmaze
            finish = finish
            enemies=enemies
            coins=Coins
            score = Config.punteggio
            path_color = Color.Black

        },match key with None -> false | Some k -> k.KeyChar = 'q'

    
    let st = { 
        player = player
        blockmaze = blockmaze
        finish = finish
        enemies=Enemies
        coins=Coins
        score = Config.punteggio
        path_color = Color.Black
    }
    
    //here I just  print my identification
    let Cerificate = engine.create_and_register_sprite (image.rectangle (W, H, pixel.filled Color.Black, pixel.filled Color.Black),2,H+3,4)
    Cerificate.draw_text("\n\n\nUse W A S D to move.\nPress (space) to activate the drill\nPress (q) to exit.\n(#)Enemies\n(*)Coins",1, 1, Color.White, Color.Black)
    
    engine.loop my_update st
 














