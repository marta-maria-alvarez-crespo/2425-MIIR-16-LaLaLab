
module perfil_l(posicion = [0,0,0], rotate = [0,0,0], mirror = [0,0,0], alto = 150, grosor = 5, longitud = 50, center=false) {
    // Perfil en L en vertical, primitiva de la base
    translate(posicion)
    rotate(rotate)
    mirror(mirror)
    union(){
        cube([grosor,longitud,alto], center=center);
        cube([longitud,grosor,alto], center=center);
    }
}

module soportes(largo, ancho, alto, grosor = 5, longitud = 50){
    // Soportes de la base del tanque, en vertical
        perfil_l(posicion = [0,ancho,0], mirror = [0,1,0], alto = alto, grosor=grosor, longitud= longitud, center=false);  
        perfil_l(posicion = [largo,ancho,0], mirror = [1,1,0], alto = alto, longitud= longitud,  grosor=grosor,center=false);
        perfil_l(posicion = [0,0,0], mirror = [0,0,0], alto = alto,  grosor=grosor, longitud= longitud, center=false);
        perfil_l(posicion = [largo,0,0], mirror = [1,0,0], alto = alto,  grosor=grosor, longitud= longitud, center=false);
}

module base(longitud = 50, ancho_base = 460, ancho_caja = 230, largo_base= 460*(750/460), grosor = 5){
    perfil_l(posicion = [longitud,-(ancho_base-ancho_caja)/2,0], rotate= [0,90,90], mirror = [0,0,0], alto = ancho_base, grosor = grosor, longitud= longitud, center=false);
    perfil_l(posicion = [largo_base,-(ancho_base-ancho_caja)/2,0], rotate = [0,90,270], mirror = [0,0,1], alto = ancho_base, grosor = grosor, longitud= longitud, center=false);
    perfil_l(posicion = [longitud,longitud,0], rotate= [270,0,270], mirror = [0,0,0], alto = largo_base-longitud, grosor = grosor, longitud= longitud, center=false);
    perfil_l(posicion = [longitud,ancho_caja- longitud,0], rotate = [270,0,270], mirror = [1,0,0], alto = largo_base-longitud, grosor = grosor, longitud= longitud, center=false);
}

module caja(posicion = [0,0,0], rotate = [0,0,0], mirror = [0,0,0], alto = 25, ancho = 230, largo = 50,  grosor = 3, caras = 4) {
    translate(posicion)
    rotate(rotate)
    mirror(mirror) 
    difference() {
        cube([largo,ancho,alto]);
            translate([grosor,grosor,grosor])
            cube([largo-2*grosor,ancho-2*grosor, alto]);
}}

module tanque(posicion = [0,0,0], rotate = [0,0,0], mirror = [0,0,0], alto = 25, ancho = 230, largo = 50,  grosor = 3) {
    translate(posicion)
    rotate(rotate)
    mirror(mirror) 
    difference() {
        cube([largo,ancho,alto]);
        translate([grosor,grosor,grosor])
        cube([largo-2*grosor,ancho-2*grosor, alto]);
        translate([grosor+largo/20,-ancho/2,grosor])
        cube([largo-largo/10-2*grosor,ancho-2*grosor, alto]);
    }
}

module estructura_tanques(
    longitud = 50, 
    ancho_caja = 230, 
    largo_caja_1 = 350, 
    alto_caja_1 = 150, 
    largo_caja_2 = 230, 
    alto_caja_2 = 435, 
    grosor = 5, 
    grosor_tanque = 2, 
    alto_tanque_1 = 200, 
    alto_tanque_2 = 340
){
    // Soportes de la base del tanque
    color("hotpink")
    base(longitud = longitud, ancho_base = ancho_caja+230, ancho_caja = ancho_caja, grosor = grosor, largo_base = largo_caja_2+largo_caja_1+largo_caja_1*0.1+160);
    
    // tanque 1
    soportes(largo = largo_caja_1, ancho = ancho_caja, alto = alto_caja_1, grosor = grosor,  longitud= longitud);
    caja(posicion = [-largo_caja_1*0.1/2,-ancho_caja*0.1/2,alto_caja_1], alto = 25, ancho = ancho_caja+ancho_caja*0.1, largo = largo_caja_1+ largo_caja_1*0.1, grosor = grosor_tanque);
    tanque(posicion = [-largo_caja_1*0.1/2+grosor*2,-ancho_caja*0.1/2+grosor*2,alto_caja_1+grosor], alto = alto_tanque_1, ancho = ancho_caja+ ancho_caja*0.1-grosor*4, largo = largo_caja_1+ largo_caja_1*0.1-grosor*4, grosor = grosor_tanque);
    translate([-largo_caja_1*0.1/2+grosor*2+ grosor_tanque,-ancho_caja*0.1/2+grosor*2+grosor_tanque,alto_caja_1])
    color("white", alpha = 0.3)
    cube([largo_caja_1+ largo_caja_1*0.1-grosor*4-grosor_tanque*2, grosor_tanque, alto_tanque_1+grosor]);

    // tanque 2
    translate([largo_caja_1+largo_caja_1*0.1,0,0]){
        soportes(largo = largo_caja_2, ancho = ancho_caja, alto = alto_caja_2, grosor = grosor,  longitud= longitud);
        caja(posicion = [grosor,grosor,alto_caja_2-alto_caja_2*0.13], alto = 25, ancho = ancho_caja-grosor*2, largo = largo_caja_2-grosor*2, grosor = grosor_tanque);
        tanque(posicion = [grosor+grosor_tanque,grosor+grosor_tanque,alto_caja_2-alto_caja_2*0.13+grosor_tanque], alto = alto_tanque_2, ancho = ancho_caja-grosor*2-grosor_tanque*2, largo = largo_caja_2-grosor*2-grosor_tanque*2, grosor = grosor_tanque);
        translate([grosor+grosor_tanque*2,grosor+grosor_tanque*2,alto_caja_2-alto_caja_2*0.13])
        color("white", alpha = 0.3)
        cube([largo_caja_2-grosor*2-grosor_tanque*2-grosor_tanque*2, grosor_tanque, alto_tanque_2+grosor_tanque]);
    }
}


module metacrilato(){
    union(){
        difference() {
            cube([350,5,240]);
            translate([-50,-50,240-150+1])
            cube([115+50,150,151]);
        }
        translate([0,5.1,0])
        cube([5,135,235]);
        translate([345,5.1,0])
        cube([5,135,235]);
        translate([0.1,5.1,235.1])
        cube([350,135,5]);
        translate([5.1,135,0])
        cube([339.1,5,234.6]);
        }
}


// PARÁMETROS DEL TANQUE (ACTUALMENTE)
// Perfil en L
grosor = 5;
longitud = 50;

// Soportes del tanque 1
ancho_caja = 230;
largo_caja_1 = 350;
alto_caja_1 = 150;
alto_tanque_1 = 200;

// Soportes del tanque 2
largo_caja_2 = 230;
alto_caja_2 = 435;
alto_tanque_2 = 340;

// Grosor de los tanques
grosor_tanque = 2;


// estructura_tanques(longitud = longitud, ancho_caja = ancho_caja, largo_caja_1 = largo_caja_1, alto_caja_1 = alto_caja_1, largo_caja_2 = largo_caja_2, alto_caja_2 = alto_caja_2, grosor = grosor, grosor_tanque = grosor_tanque, alto_tanque_1 = alto_tanque_1, alto_tanque_2 = alto_tanque_2);
metacrilato();