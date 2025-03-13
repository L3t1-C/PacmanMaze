(*
* LabProg2019 - Progetto di Programmazione a.a. 2019-20
* Main.fs: main code
* (C) 2019 Alvise Spano' @ Universita' Ca' Foscari di Venezia
*)

module LabProg2019.Menù

open System
open External
open Engine
open Gfx

[< NoEquality; NoComparison >]
type state={
    selectedMenù : sprite
}

let mutable MenugameMode = Config.PlayMode.Exit
let mutable MeugameSize = 25,25

//Aggiorniamo la scelta della modalità di gioco

let UpdateChoice (key : ConsoleKeyInfo ) (screen : wronly_raster) (inf : info) (st : state) =
    // Confronto i tasti premuti per associarli alla modalità di gioco
    match key.KeyChar with 
        |'1'->MenugameMode<- Config.PlayMode.OnePlayer
        |'2'->MenugameMode<- Config.PlayMode.Automatic
        |_->failwith"Invalid choice"

    //Se preme il tasto Q devo uscire
    if key.KeyChar = 'q' || key.KeyChar = 'Q' then MenugameMode <- Config.PlayMode.Exit
    //se vengono premuti i caratteri 'q' e '1-2'
    //esco dal loop(il while true nel main) e passo al main per l'esecuzione del gioco
    if key.KeyChar = 'q' || key.KeyChar = 'Q' || key.KeyChar = '1' || key.KeyChar = '2' then st,true else st,false

    //Aggiorno anche le dimensioni che scelgo per giocare
     
let updateSizeScreen (key : ConsoleKeyInfo ) (screen : wronly_raster) (inf : info) (st : state) =     
     match key.KeyChar with 
        | '1' -> MeugameSize <- (15,15)
        | '2' -> MeugameSize <- (25,25)
        | '3' -> MeugameSize <- (45,45) //Ho messo questa grandezza perchè ho lo schermo piccolo, nelle grandezze superiori non vedo la fine del maze
        | _ -> failwith "Error, wrong choice for the resolution"

     //salvo lo stato 'exit' come prima
     if key.KeyChar = 'q' || key.KeyChar = 'Q' then MeugameSize <- (-1,-1)
        //se vengono premuti i caratteri '1-2-3' e 'q' esco dal loop e passo al main per l'esecuzione del gioco
     if key.KeyChar = 'q' || key.KeyChar = 'Q' || key.KeyChar = '1' || key.KeyChar = '2' || key.KeyChar = '3' then st,true else st,false


     //Scrivo il main del menù principale
let main (W,H) = 
    //imposto un valore standard
    MeugameSize <- 25,25
    let engine = new engine (W, H)

    //creo lo sfondo del menu' utilizzando un quadrato gia' supportato dal motore
    let menu = engine.create_and_register_sprite(image.rectangle (W-2, H/2, pixel.filledMenu Color.Cyan, pixel.filledMenu Color.White),2,H/4,0)
   
    // (image.circle (1, pixel.filled Color.DarkCyan, pixel.filled Color.Gray), W / 2, H / 2, 1)

    menu.draw_text("Game_Mode\n\n1)Interactive_maze\n2)Automatic_maze_resolution\n\n\nPress(1)or(2)_to_enter_Press_(q)_to_quit\n", 2, 1, Color.Black, Color.White)
       
    let st0 = {selectedMenù = menu}
    
    //avvio il key loop che rimane in attesa della pressione di un tasto
    
    engine.loop_on_key   UpdateChoice st0
    
    //quando esco dal loop elimino gli sprite creati
    menu.clear;
  
    //vado al menu della grandezza schermo se l'utente non esce
    if MenugameMode <> Config.PlayMode.Exit then
       let menu = engine.create_and_register_sprite(image.rectangle (W-2, H/2, pixel.filledMenu Color.Cyan, pixel.filledMenu Color.White),2,H/4,0)
       let st0 = {selectedMenù = menu}
       menu.draw_text("Choose_the_Resolution\n\n1)15*15\n2)25*25\n3)45*45\n\nPress(1)or(2)or(3)_to_enter_Press_(q)_to_quit\n", 2, 1, Color.Black, Color.White)
   
       engine.loop_on_key  updateSizeScreen st0

