(*
* LabProg2019 - Progetto di Programmazione a.a. 2019-20
* FinishScreen.fs: FinishScreen code
*(C) 882713 Cantoni Letizia Gruppo:LabProg2019B_28  @ Universita' Ca' Foscari di Venezia
*)

module LabProg2019.FinishScreen

open System
open Engine
open Gfx


let W = 50 //define the width and the height of my screen just in case something goes wrong
let H = 25
let mutable MenugameMode = Config.PlayMode.Exit

[< NoEquality; NoComparison >]
type state = 
    {
        text : sprite
    }

let engine = new engine (W, H)


//when i recive playerWin, if it is true, it means that someone win else, if it is false, somone lose
let FinishResult (playerWin:bool) = 
    Console.Clear()
    let W=50//define the width and the height of my screen 
    let H=25
    let engine = new engine (W, H)
    //I control if the player wants to exit
    let my_update (key : ConsoleKeyInfo ) (screen : wronly_raster) (inf : info) (st : state) =    
        if (key.KeyChar = 'q' || key.KeyChar = 'Q') then
            exit 0
            st,true
        else
            st,false

  
    //the sprite for my  text
    let menu = engine.create_and_register_sprite(image.rectangle (W, H/2+7, pixel.filled Color.DarkCyan, pixel.filled Color.Black),0,H/2-10,2)

    
    let mutable d=4
    let c=1
    if playerWin then 
        //this is just for the pacman mode, because if you win, i print also the score you made.
        if (Config.finishpac=1) then 
            Console.Clear()
            menu.draw_text("........***............*.............***.......", c, d+1, Color.DarkGray, Color.Black)
            menu.draw_text(".........*........../^\_/^\...........*........", c, d+2, Color.Gray, Color.Black)
            menu.draw_text("...............*...( ^ . ^ )....*..............", c, d+3, Color.Gray, Color.Black)
            menu.draw_text("......***...........>  -  <...........***......", c, d+4, Color.Gray, Color.Black)
            menu.draw_text(".......*........*..............*.......*.......", c, d+5, Color.DarkGray, Color.Black)
            menu.draw_text("YOU WIN!", W/3, d+6, Color.Cyan, Color.Black)
            menu.draw_text(sprintf"TOTAL SCORE %d/20 (^_+)" Config.punteggio , W/3-2, d+7, Color.Green, Color.Black)
            menu.draw_text("\nPress (q) to esc ",  W/3, d+8, Color.White, Color.Black)
            

       //this is when you win
        menu.draw_text("........***............*.............***.......", c, d+1, Color.DarkGray, Color.Black)
        menu.draw_text(".........*........../^\_/^\...........*........", c, d+2, Color.Gray, Color.Black)
        menu.draw_text("...............*...( ^ . ^ )....*..............", c, d+3, Color.Gray, Color.Black)
        menu.draw_text("......***...........>  -  <...........***......", c, d+4, Color.Gray, Color.Black)
        menu.draw_text(".......*........*..............*.......*.......", c, d+5, Color.DarkGray, Color.Black)
        menu.draw_text("                    FINISH! ", c, d+6, Color.Cyan, Color.Black)
        menu.draw_text("\nPress (q) to esc ", W/3, d+8, Color.White, Color.Black)



    else
      //this is when you lose
       menu.draw_text("................................................", c, d+1, Color.DarkGray, Color.Black)
       menu.draw_text("..................../^\_/^\.....................", c, d+2, Color.Gray, Color.Black)
       menu.draw_text("...................( =o.o= )....................", c, d+3, Color.Gray, Color.Black)
       menu.draw_text("....................>  -  <.....................", c, d+4, Color.Gray, Color.Black)
       menu.draw_text("................................................", c, d+5, Color.DarkGray, Color.Black)
       menu.draw_text("                   GAME OVER! ", c, d+6, Color.Red, Color.Black)
       menu.draw_text("\nPress (q) to esc ", W/3, d+8, Color.White, Color.Black)
    
    let st0 = {
        text = menu
    }
    
    //Start the key loop
    //it waits for a key to be pressed
    engine.loop_on_key  my_update st0
    
  