(*
* LabProg2019 - Progetto di Programmazione a.a. 2019-20
* Maze.fs: maze
* (C) 882713 Cantoni Letizia Gruppo:LabProg2019B_28  @ Universita' Ca' Foscari di Venezia
*)

module LabProg2019.Maze

open System
open External
open Gfx
open Engine
//open FinishScreen    

//implementation of Prim's algorithm

let mutable W,H = 50,25//i assign to the screen a width and a height just in case something goes wrong

Engine.Hide_FPS <- false//this is to not show the frame count and the timer 

[< NoEquality; NoComparison >]
type state = {
    player : sprite
    blockmaze : sprite
    finish : sprite
    path_color: Color
}

type CharInfo with
    static member wall = pixel.create (Config.wall_pixel_char, Color.DarkGray)
    static member internal path = pixel.filled Color.White
    member this.isWall = this = pixel.wall

let rand = new System.Random()

type Direction = | Ovest | Est | Nord | Sud | NULL
type Visit = Visited | NotVisited
type Cell = | Blocked | Passage

type Maze  = 
    { 
        Grid : Cell[,]
        Width : int
        Height : int
        Visited : Visit[,]
    }
//this set all the array with a wall.(Every cell is a wall)
let initializationMaze H W = 
    { 
        Grid = Array2D.init H W (fun _ _ -> Blocked) 
        Width = H
        Height = W
        Visited = Array2D.init H W (fun _ _ -> NotVisited)
    }
//this function generate the maze
let generate (maze : Maze) : Maze =
//this function return if a point(two coordinate) is in the maze or not
    let IntheMaze (x,y) = x>0 && x < maze.Width-1 && y>0 && y<maze.Height-1
//this function return the list of wall near (in two cell) a cell    
    let NearWallList (x,y) =[x-2,y;x+2,y; x,y-2; x, y+2]|> List.filter (fun (x,y) -> IntheMaze (x,y) && maze.Grid.[x,y] = Blocked)
//this function return the list of free Passage near (in two cell) a cell       
    let neighbour2 (x,y) =[x-2,y;x+2,y; x,y-2; x, y+2]|> List.filter (fun (x,y) -> IntheMaze (x,y) && maze.Grid.[x,y] = Passage)
//this function return two random coordinate(a cell) in the maze    
    let randomCell () = rand.Next(maze.Width),rand.Next(maze.Height)   
//this function is used to remove an index from a list 
    let removeUsed index (lst : (int * int) list) : (int * int) list =
        let x,y = lst.[index]
        lst |> List.filter (fun (a,b) -> not (a = x && b = y))
//this function return the intermediate cell between other two    
    let between p1 p2 =
        let x = 
            match (fst p2 - fst p1) with
            | 0 -> fst p1
            | 2 -> 1 + fst p1
            | -2 -> -1 + fst p1
            | _ -> failwith "ERROR BETWEEN()"
        let y = 
            match (snd p2 - snd p1) with
            | 0 -> snd p1
            | 2 -> 1 + snd p1
            | -2 -> -1 + snd p1
            | _ -> failwith "ERROR BETWEEN()"
        (x,y)

//this function + the recursive function below create the paths in the maze

    let connectRandomNeighbor (x,y) = 
        let neighbors = neighbour2 (x,y)
        let pickedIndex = rand.Next(neighbors.Length)
        let xn,yn = 
            try
            neighbors.[pickedIndex]
            with
            | :? System.ArgumentException -> neighbors.[rand.Next(neighbors.Length)]
        let xb,yb = between (x,y) (xn,yn)
        maze.Grid.[xb,yb] <- Passage
        ()

    let rec recursivefindepoint front =
        match front with
        | [] -> ()
        | _ ->
            let pickedIndex = rand.Next(front.Length)
            let xf,yf = front.[pickedIndex]
            maze.Grid.[xf,yf] <- Passage
            connectRandomNeighbor (xf,yf)
            recursivefindepoint ((front |> removeUsed pickedIndex) @ NearWallList (xf,yf))

    let rec FirstCell (x,y) =
        let maxx = maze.Width
        let maxy = maze.Height
//check that the coordinate are in a correct position
        match x,y with
        | 0,_ -> FirstCell (randomCell())
        | _,0 -> FirstCell (randomCell())
        | x,y -> if x%2=0 || y%2=0 || x=maxx || y=maxy then FirstCell (randomCell()) else x,y
    
//here i select (random) the start point
    let x,y = FirstCell (randomCell())
//it has to be a Passage
    maze.Grid.[x,y] <- Passage

//here i start the generation
    recursivefindepoint (NearWallList (x,y))

    maze

///maze generate is a Shortcut for every time that i want to call the generation
let mutable MazeGenerate = generate(initializationMaze W H)


//Here i move the player if the direction that i choose is possible
let moveplay (key : ConsoleKeyInfo) (screen : wronly_raster)(inf:info)(st : state)=
    let isWall (x,y) =
           if MazeGenerate.Grid.[int (st.player.x + x), int (st.player.y + y)] = Passage then               
               st.player.move_by (x,y)
           else st.player.move_by (0.,0.)

    match key.KeyChar with 
    |'w'|'W' -> isWall(0.,-1.)
    |'s'|'S' -> isWall(0., 1.)
    |'a'|'A' -> isWall(-1., 0.)
    |'d'|'D' -> isWall(1.,0.)
    |'q'|'Q'-> exit 0
    | _ -> st.player.move_by (0., 0.)
     
//when i get to the end i call an animation in the file FinishScreen
    if st.player.x = st.finish.x && st.player.y = st.finish.y then 
        FinishScreen.FinishResult(true)
        st, true
    else
        st,(key.KeyChar = 'q' || key.KeyChar = 'Q') 
        

        

let main(widthchoose,heightchoose) =   
    System.Console.Clear()
    //i had some problem with the refresh in the different game mode so, before i generate the maze i clear the screen.
     
    let mutable engine = new engine (W, H)
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
   

//here i create the player sprite and the finish sprite
    let player = engine.create_and_register_sprite (image.rectangle (1, 1, pixel.filled Color.DarkBlue,pixel.filled Color.DarkBlue),1, 1, 2)
    let finish = engine.create_and_register_sprite (image.rectangle (1, 1, pixel.filled Color.DarkRed,pixel.filled Color.DarkRed), W-3, H-2, 2) 


    let st = { 
        player = player
        blockmaze = blockmaze
        finish = finish
        path_color = Color.White
    }

//here I just  print my identification
    let Cerificate = engine.create_and_register_sprite (image.rectangle (W, H, pixel.filled Color.Black, pixel.filled Color.Black),2,H,4)
    Cerificate.draw_text("Use W A S D to move.\nPress (q) to exit.\n\n(C)882713 Cantoni\nGruppo:LabProg2019B_28 ",1, 1, Color.White, Color.Black)
   //Start the key loop
   //it waits for a key to be pressed 
    engine.loop_on_key moveplay st



    