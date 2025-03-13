(*
* LabProg2019 - Progetto di Programmazione a.a. 2019-20
* Menù.fs: menù code
*(C) 882713 Cantoni Letizia Gruppo:LabProg2019B_28  @ Universita' Ca' Foscari di Venezia
*)

module LabProg2019.Menù

open System
open External
open Engine
open Gfx

[< NoEquality; NoComparison >]
type state = 
    {
        selectedMenù : sprite //this is the sprite with the menù and all the choice
        Certification : sprite //this is the sprite where i put my identification
    }

let mutable MenugameMode = Config.PlayMode.Exit //i assign to the game mode the exit just in case something goes wrong
let mutable MeugameSize = 50,25 //i assign to the game size a width and a height just in case something goes wrong

//Here i associate the choice of the player with the different game choices
let UpdateChoice (key : ConsoleKeyInfo ) (screen : wronly_raster) (inf : info) (st : state) =
    
    match key.KeyChar with 
        |'1'->MenugameMode<- Config.PlayMode.Player
        |'2'->MenugameMode<- Config.PlayMode.Automatic
        |'3'->MenugameMode<- Config.PlayMode.InvisibleAutores
        |'4'->MenugameMode<- Config.PlayMode.Pacman
        | 'q' -> exit 0
        |_->failwith"Invalid choice"

    //If it is pressed the Q key the screen must close
    if key.KeyChar = 'q' || key.KeyChar = 'Q' then MenugameMode <- Config.PlayMode.Exit
    
    //exit from the loop (while true in the main) and then i start the game
    if key.KeyChar = 'q' || key.KeyChar = 'Q' || key.KeyChar = '1' || key.KeyChar = '2'|| key.KeyChar = '3'|| key.KeyChar = '4' then st,true else st,false


//Here i associate the choice of the player with the different game sizes    
let updateSizeScreen (key : ConsoleKeyInfo ) (screen : wronly_raster) (inf : info) (st : state) =     

     match key.KeyChar with 
        | '1' -> MeugameSize <- (30,15)
        | '2' -> MeugameSize <- (50,25)
        | '3' -> MeugameSize <- (90,45) 
        | 'q' -> exit 0
        | _ -> failwith "Error, wrong choice for the resolution"

     //If it is pressed the Q key the screen must close
     if key.KeyChar = 'q' || key.KeyChar = 'Q' then MeugameSize <- (0,0)
     //exit from the loop (while true in the main) and then i start the game
     if key.KeyChar = 'q' || key.KeyChar = 'Q' || key.KeyChar = '1' || key.KeyChar = '2' || key.KeyChar = '3' then st,true else st,false



let main (W,H) = 
    //define the width and the height of my screen 
    MeugameSize <- 50,25
    let engine = new engine (W, H)
    
    //sprite(background) for the menù
    let menu = engine.create_and_register_sprite(image.rectangle (W-2, H/2, pixel.filledMenu Color.Cyan, pixel.filledMenu Color.White),1,H/4,0)
    //sprite(background) for my identification
    let Cerificate = engine.create_and_register_sprite (image.rectangle (W-4, 7, pixel.filled Color.Black, pixel.filled Color.Black),1,H-3,0)
    
    let  a=2
    let mutable b=1

    menu.draw_text("Game_Mode", a, b, Color.Black, Color.White)
    menu.draw_text("1)Interactive_maze", a, b+2, Color.Black, Color.White)
    menu.draw_text("2)Automatic_maze_resolution", a, b+3, Color.Black, Color.White)
    menu.draw_text("3)Invisible_Automatic_Resolution", a, b+4, Color.Black, Color.White)
    menu.draw_text("4)Pacman",a, b+5, Color.Black, Color.White)
    menu.draw_text("Press(1)or(2)or(3)_to_enter\nPress_(q)_to_quit\n",a, b+7, Color.Black, Color.White)
    Cerificate.draw_text("(C) 882713 Cantoni Letizia\n Gruppo:LabProg2019B_28 ",a, 1, Color.White, Color.Black)
    
    let st0 = {
        selectedMenù = menu
        Certification=Cerificate
    }
    
    //Start the key loop
    //it waits for a key to be pressed 
    engine.loop_on_key   UpdateChoice st0
    
    Cerificate.clear;//i clear because i use the same sprites also for the game size choice
    menu.clear;
  
    //if the player didn't exit it goes to the size choice
    if MenugameMode <> Config.PlayMode.Exit then
       let menu = engine.create_and_register_sprite(image.rectangle (W-2, H/2, pixel.filledMenu Color.Cyan, pixel.filledMenu Color.White),1,H/4,0)
       let Cerificate = engine.create_and_register_sprite (image.rectangle (W-4, 7, pixel.filled Color.Black, pixel.filled Color.Black),1,H-3,0)
       
       let st0 = {
            selectedMenù = menu
            Certification=Cerificate
       }

       menu.draw_text("Choose_the_difficulty", a, b, Color.Black, Color.White)
       menu.draw_text("1)Easy", a, b+2, Color.Black, Color.White)
       menu.draw_text("2)Medium", a, b+3, Color.Black, Color.White)
       menu.draw_text("3)Difficult", a, b+4, Color.Black, Color.White)
       menu.draw_text("Press(1)or(2)or(3)_to_enter\nPress_(q)_to_quit\n", a, b+6, Color.Black, Color.White)
       Cerificate.draw_text("(C) 882713 Cantoni Letizia\n Gruppo:LabProg2019B_28 ",a, 1, Color.White, Color.Black)
       
       engine.loop_on_key  updateSizeScreen st0

       Cerificate.clear;
       menu.clear;
       