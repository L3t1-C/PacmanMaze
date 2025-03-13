(*
* LabProg2019 - Progetto di Programmazione a.a. 2019-20
* BeforeMenu.fs: It's like a screensaver, before the Menù
* (C) 882713 Cantoni Letizia Gruppo:LabProg2019B_28 @ Universita' Ca' Foscari di Venezia
*)

//I wanted to use a portion of the code that had already been given to us (demo1), to create a Screensaver


module LabProg2019.BeforeMenu
open System
open Engine
open Gfx

[< NoEquality; NoComparison >]
type state = {
    sprites : (sprite * int * int)[]  
    sprites1 : (sprite * int * int)[]  
}
//define the width and the height of my screen 
let W = 50
let H = 25
let mutable a=0
let mutable b=0

let main () =       
    let engine = new engine (W, H)
    System.Console.Clear()
    //I create the background for my animation "F#"
    let menubox=engine.create_and_register_sprite (image.rectangle (W, H, pixel.filled Color.Black, pixel.filled Color.Black), 0, 0, 0)
    //Here i move the sprites that create the word F#
    let my_update (key : ConsoleKeyInfo option) (screen : wronly_raster) (inf : info) (st : state)  =
        
        //I need to move this sprites to make the others (that create the word F#) to move at the same time in the same place.
        let sprites1 = [| 
            for spr1, dx, dy in st.sprites1 do 
            let a = dx
            let b = dy
            spr1.move_by (dx, dy)
            let dx = if int spr1.x + spr1.width >=  W || int spr1.x<= 0 then -dx else dx
            let dy = if int spr1.y + spr1.height  >= H-4 || int spr1.y <= 0 then -dy else dy
         
            yield spr1, dx, dy
            |]
        //I move the sprites according to the sprites1
        let sprites = [|
            for spr1,dx1,dy1 in st.sprites1 do
                for spr, dx, dy in st.sprites do  
                let dx=dx1
                let dy=dy1
                spr.move_by (dx, dy)               
                yield spr, dx, dy
            |]

        menubox.draw_text ("Per andare al Menu'\nPremi il tasto (q) " ,W/3, H-4, Color.DarkCyan, Color.Black)
        
        // calculate next state
        { 
        sprites = sprites
        sprites1 = sprites1            
        },match key with None -> false | Some k -> k.KeyChar = 'q'
             
    //Here i create the word F#
    let sprites = [|
        
            
            let spr = engine.create_and_register_sprite (image.rectangle (6,1, pixel.filledSubMenu), 5, 3, 1)
            let d () = 1
            yield spr, d (), d ()
            
            let spr = engine.create_and_register_sprite (image.rectangle (1,6, pixel.filledSubMenu), 5, 3, 1)
            let d () = 1
            yield spr, d (), d ()

            let spr = engine.create_and_register_sprite (image.rectangle (5,1, pixel.filledSubMenu), 5, 5, 1)
            let d () = 1
            yield spr, d (), d ()    

            let spr = engine.create_and_register_sprite (image.rectangle (1,5, pixel.filledSubMenu), 13, 4, 1)
            let d () = 1
            yield spr, d (), d ()
             
            let spr = engine.create_and_register_sprite (image.rectangle (1,5, pixel.filledSubMenu), 18, 4, 1)
            let d () = 1
            yield spr, d (), d () 

            let spr = engine.create_and_register_sprite (image.rectangle (8,1, pixel.filledSubMenu), 12, 5, 1)            
            let d () = 1
            yield spr, d (), d ()

            let spr = engine.create_and_register_sprite (image.rectangle (8,1, pixel.filledSubMenu), 12, 7, 1)
            let d () = 1
            yield spr, d (), d ()

        |]
    //here i create a sprite as big as the word f# in oreder to make then move exactly
    let sprites1 = [|
        let spr1 = engine.create_and_register_sprite (image.rectangle (18,9, pixel.fillball Color.Black, pixel.fillball Color.Black), 3, 1, 1)
        let d () = 1
        yield spr1, d (), d ()
        |]
    
    
    // initialize state
    let st0 ={ 
        sprites = sprites
        sprites1=sprites1
        }
    // start engine
    engine.loop my_update st0
