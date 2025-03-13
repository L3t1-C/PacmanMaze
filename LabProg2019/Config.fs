(*
* LabProg2019 - Progetto di Programmazione a.a. 2019-20
* Config.fs: static configuration
* (C) 882713 Cantoni Letizia Gruppo:LabProg2019B_28  @ Universita' Ca' Foscari di Venezia
*)

module LabProg2019.Config

open Prelude

let filled_pixel_char = '\219' //ascii '\219' for character '█' 254 for the square
let wall_pixel_char = '\219'
let empty_pixel_char = ' '
let filled_pix_menu ='\178' //used for the menù background
let filled_sub_menu='\206'  //used for F# animation (BeforeMenù)
let fillghost= '#' // '\158' character for the enemies(Pacman)
let fillball='*' //character for the coins(Pacman)
let WallForPacman = '\177' //character for the wall in(Pacman)

let fillpacman='\184' //character for the Pacman (Pacman)

let mutable punteggio = 0 //to save the score in the pacman mode 
let mutable boool=0//to activate the drill mode
let mutable trapano_attivato= -25.//to see when to stop the drill mode and start the cooldown

let mutable finishpac=0//to see when someone finish the pacman
let mutable InvisibleAuto=0//to see if the player selected the gamemode InvisibleAutores

let default_flip_queue = 2  // double buffering
let default_fps_cap = 30

let log_pipe_name = "LogPipe"
let log_pipe_translate_eol = '\255'
let game_console_title = "Game Window"
let log_console_title = "Log Window"

let log_msg_color = Color.Gray
let log_warn_color = Color.Yellow
let log_error_color = Color.Red
let log_debug_color = Color.Cyan


type PlayMode = | Player | Automatic | InvisibleAutores | Pacman | Exit 