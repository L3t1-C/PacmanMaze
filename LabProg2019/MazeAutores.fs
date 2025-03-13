(*
* LabProg2019 - Progetto di Programmazione a.a. 2019-20
* Maze.fs: mazeAutoRes
* (C) 882713 Cantoni Letizia Gruppo:LabProg2019B_28  @ Universita' Ca' Foscari di Venezia
*)

module LabProg2019.MazeAutores

open System
open Gfx
open Engine
open System.Threading
open Maze

let mutable W,H = 50,25//i assign to the screen a width and a height just in case something goes wrong
let mutable engine = new engine (W, H)
Engine.Hide_FPS <- false//this is to not show the frame count and the timer

let mutable finishmaze = false//a boolen to see if the resolution is finish

//implementation of DFS depth first search algorithm

let PathForAutoRes (st:state) color char z =
    let fillpath = pixel.create(char, color)// if i already vivited the cell i can change the color
    ignore <| engine.create_and_register_sprite (image.rectangle (1, 1, fillpath), int (st.player.x), int (st.player.y), z)


//this function is used to move the player and to color the path if the movement is possible
let moveplay (direction:Direction)(st : state)=
    //this contols if a cell is a passage, and then move the player in tat cell
    let isWall (x,y) =
           if MazeGenerate.Grid.[int (st.player.x + x), int (st.player.y + y)] = Passage then 
               PathForAutoRes st st.path_color Config.filled_pixel_char 2
               st.player.move_by (x,y)
           else st.player.move_by (0.,0.) 
    
    match direction with 
    |Nord -> isWall(0.,-1.)
    |Sud -> isWall(0., 1.)
    |Ovest -> isWall(-1., 0.)
    |Est -> isWall(1.,0.)
    | _ -> st.player.move_by (0., 0.)
    
    //if the player arrives at the finish
    if st.player.x = st.finish.x && st.player.y = st.finish.y then 
        FinishScreen.FinishResult(true)
        st, true
    else
        st, false

//this function check if you can move in one direction
let OneStepTry (stat:state) (direction:Direction) =
    let isWall (x,y) =
        if MazeGenerate.Grid.[int (stat.player.x + x), int (stat.player.y + y)] = Passage then x,y else 0.,0.
    
    let directionmove =
            match direction with
            | Direction.Nord-> isWall(0.,-1.)
            | Direction.Sud-> isWall(0., 1.)
            | Direction.Est-> isWall(1.,0.)
            | Direction.Ovest-> isWall(-1., 0.)
            | Direction.NULL-> 0.,0.
    directionmove


let AutoResolver st screen = 

//this function move the player in the chosen direction 

    let move (stat:state) (direction:Direction) =
        let dx= stat.player.x
        let dy= stat.player.y
      
        let st, trydire =
                match direction with
                | Direction.Ovest->  moveplay Ovest stat
                | Direction.Est->  moveplay Est stat
                | Direction.Nord->  moveplay Nord stat
                | Direction.Sud->  moveplay Sud stat
                | Direction.NULL->  (stat, false)
        //aggiornamento dello schermo
        engine.refresh st false
        if stat.player.x = dx && stat.player.y = dy then
            (trydire, false)
        else 
            (trydire, true)
   
    
    //This function is used to use to try every direction and save in an array the cell who are already visited

    let rec findexit (st:state) (screen: wronly_raster) (dx,dy) =
        let wait = 50
        let muoviovunque(direction:Direction)=
            if not finishmaze then
                let tdx, tdy = OneStepTry st direction //returns the coordinate of the direction 
                //if the coordinate are different from 0.,0. (this means that there isn't a wall)
                //and the cell is not visited
                if ((tdx,tdy) <> (0.,0.)) && (MazeGenerate.Visited.[(int st.player.x + int tdx),int st.player.y + int tdy] <> Visited) then
                    Thread.Sleep(wait) //this is just to see the animation better
                    //then turn the cell into visited
                    MazeGenerate.Visited.[int st.player.x,int st.player.y] <- Visited
                    //move in that cell
                    let movement, bool = move st direction  
                    if not movement then
                        findexit st screen (tdx,tdy)
                    else
                        finishmaze <- true    //exit
        //try every direction
        for i = 0 to 3 do
            match i with
            | 0 -> muoviovunque Ovest
            | 1 -> muoviovunque Est
            | 2 -> muoviovunque Sud
            | 3 -> muoviovunque Nord
            | _ -> failwith"error"

//This block change the color of the path, cause if you arrive here you had already visit the cell
//and then it goes back
        if not finishmaze then
            Thread.Sleep(wait)
            PathForAutoRes st Color.DarkGreen Config.filled_pixel_char 4
            st.player.move_by(-dx,-dy)
            engine.refresh st false
//Call again the function
    findexit st screen (0.,0.)
    
///this function enables the automatic start
let automatic_start (key : ConsoleKeyInfo) (screen : wronly_raster) (inf:info) (st : state) : (state*bool) = 
    match key.KeyChar with 
    |'s'|'S' -> AutoResolver st screen             
                st, true
    |'q'|'Q' -> exit 0
    | _   -> st, false



let main(widthchoose,heightchoose) = 

    W <- widthchoose
    H <- heightchoose

    engine <- new engine (W, H+7)
    
    MazeGenerate <- generate(initializationMaze W H)
    //this function prints the maze
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
    //this is the background sprites(for the maze)
    let blockmaze= engine.create_and_register_sprite (new image (W,H,(printmaze MazeGenerate)), 0, 0, 0)
    
    if (Config.InvisibleAuto=1) then//this is for the game mode "Invisible automatic resolution"
        blockmaze.clear

//here i create the player sprite and the finish sprite    
    let player = engine.create_and_register_sprite (image.rectangle (1, 1, pixel.filled Color.DarkBlue,pixel.filled Color.DarkBlue),1, 1, 2)
    let finish = engine.create_and_register_sprite (image.rectangle (1, 1, pixel.filled Color.DarkRed,pixel.filled Color.DarkRed), W-3, H-2, 2) 


    let st0 = { 
        player = player
        blockmaze = blockmaze
        finish = finish
        path_color = Color.Cyan
    }

    finishmaze <- false
    //here I just  print my identification
    let Cerificate = engine.create_and_register_sprite (image.rectangle (W, H/2, pixel.filled Color.Black, pixel.filled Color.Black),2,H,4)
    Cerificate.draw_text("Press (S) to solve the maze.\nPress (q) to exit.\n\n(C)882713 Cantoni\nGruppo:LabProg2019B_28 ",1, 1, Color.White, Color.Black)
        
    engine.loop_on_key automatic_start st0 

















