use <estructura_planta.scad>;
use <electronica.scad>;

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



module main(){
    // metal
    color("gray"){
    echo(largo_caja_1+30+largo_caja_2+100); // 710
    echo(largo_caja_2+largo_caja_1+largo_caja_1*0.1+160); // 775
    base(longitud = longitud, ancho_base = ancho_caja+230, ancho_caja = ancho_caja, grosor = grosor, largo_base = largo_caja_1+30+largo_caja_2+100);
    soportes(largo = largo_caja_1, ancho = ancho_caja, alto = alto_caja_1, grosor = grosor,  longitud= longitud);
    caja(posicion = [-largo_caja_1*0.1/2,-ancho_caja*0.1/2,alto_caja_1], alto = 25, ancho = ancho_caja+ancho_caja*0.1, largo = largo_caja_1+ largo_caja_1*0.1, grosor = grosor_tanque);
    tanque(posicion = [-largo_caja_1*0.1/2+grosor*2,-ancho_caja*0.1/2+grosor*2,alto_caja_1+grosor], alto = alto_tanque_1, ancho = ancho_caja+ ancho_caja*0.1-grosor*4, largo = largo_caja_1+ largo_caja_1*0.1-grosor*4, grosor = grosor_tanque);
    translate([largo_caja_1+largo_caja_1*0.1,0,0]){
        soportes(largo = largo_caja_2, ancho = ancho_caja, alto = alto_caja_2, grosor = grosor,  longitud= longitud);
        caja(posicion = [grosor,grosor,alto_caja_2-alto_caja_2*0.13], alto = 25, ancho = ancho_caja-grosor*2, largo = largo_caja_2-grosor*2, grosor = grosor_tanque);
        tanque(posicion = [grosor+grosor_tanque,grosor+grosor_tanque,alto_caja_2-alto_caja_2*0.13+grosor_tanque], alto = alto_tanque_2, ancho = ancho_caja-grosor*2-grosor_tanque*2, largo = largo_caja_2-grosor*2-grosor_tanque*2, grosor = grosor_tanque);
    }}
    
    // cristal
    color("white", alpha = 0.3){
    translate([-largo_caja_1*0.1/2+grosor*2+ grosor_tanque,-ancho_caja*0.1/2+grosor*2+grosor_tanque,alto_caja_1])
    cube([largo_caja_1+ largo_caja_1*0.1-grosor*4-grosor_tanque*2, grosor_tanque, alto_tanque_1+grosor]);
    translate([largo_caja_1+largo_caja_1*0.1+grosor+grosor_tanque*2, grosor+grosor_tanque*2, alto_caja_2-alto_caja_2*0.13+grosor_tanque])
    cube([largo_caja_2-grosor*2-grosor_tanque*2-grosor_tanque*2, grosor_tanque, alto_tanque_2]);
    translate([largo_caja_1+grosor+grosor_tanque*2, -140, alto_caja_1+25]) 
    metacrilato();
    }
    
    //metal 2
    color("grey")
    translate([340/2+largo_caja_1+15,-8,230])
    rotate([180,90,0])  
    rack(l = 340);

    color("grey")
    translate([340/2+largo_caja_1+15,-8,335])
    rotate([180,90,0])  
    rack(l = 340);
    
    for (i = [1:2:3]) {
        translate([largo_caja_1+15,0,190]){
            // rotate([0,0,-90])
            // modulo_interruptor(ancho = 20, radio_eje = 6);
            translate([10*i,-65,35.2])
            rotate([0,90,0]) rotate([0,0,-15]) palanca_interruptor(ancho = 20, radio_eje = 6);
        }
    }

    translate([largo_caja_1+15+20,0,190])
    rotate([0,0,-90])
    modulo_interruptor(ancho = 20, radio_eje = 6);


    for (i = [1.2:2:27]) {
            difference(){
                union() {
                translate([largo_caja_1+62+2.8*i,-6,203]){
                    translate([0,-18,10])
                    rotate([0,90,0]) 
                    cylinder(h = 5, r = 5);
                    translate([0,-18,40])
                    rotate([0,90,0]) 
                    cylinder(h = 5, r = 5);
                    rotate([0,0,-90])
                    scale([0.4,1,0.65]) 
                    modulo_simple(ancho = 5);}
                }
                union(){
                    translate([largo_caja_1+62+2.8*i,-6,203]){
                        translate([-0.1,-30,29])
                        rotate([0,90,0]) 
                        cube([5,10,5.2]);
                        
                        translate([2.6,-22,35], $fn = 20)
                        rotate([90,0,0])
                        cylinder(h = 5, r = 1.5);

                        translate([2.6,-22,34-17], $fn = 20)
                        rotate([90,0,0])
                        cylinder(h = 5, r = 1.5);
                    }
                }
        }
    }

    for (i = [49.7:2:70]) {
        difference(){
            union() {
            translate([largo_caja_1+62+2.8*i,-6,203]){
                translate([0,-18,10])
                rotate([0,90,0]) 
                cylinder(h = 5, r = 5);
                translate([0,-18,40])
                rotate([0,90,0]) 
                cylinder(h = 5, r = 5);
                rotate([0,0,-90])
                scale([0.4,1,0.65]) 
                modulo_simple(ancho = 5);}
            }
            union(){
                translate([largo_caja_1+62+2.8*i,-6,203]){
                    translate([-0.1,-30,29])
                    rotate([0,90,0]) 
                    cube([5,10,5.2]);
                    
                    translate([2.6,-22,35], $fn = 20)
                    rotate([90,0,0])
                    cylinder(h = 5, r = 1.5);

                    translate([2.6,-22,34-17], $fn = 20)
                    rotate([90,0,0])
                    cylinder(h = 5, r = 1.5);
                }
            }
        }
    }

    for (i = [1.2:2:5]) {
        color("red")
        translate([largo_caja_1+49.5+5.2*i,-6,198.5]){
            rotate([0,0,-90])
            scale([0.45,1,0.75])
            modulo_simple(ancho = 10);
        }
    }

    for (i = [22.8:4:35]) {
        color("red")
        translate([largo_caja_1+49.5+3.9*i,-6,198.5]){
            rotate([0,0,-90])
            scale([0.45,1,0.75])
            modulo_simple(ancho = 15);
        }
    }

    color("red")
    translate([largo_caja_1+49.5+3.9*62,0,190]){
        rotate([0,0,-90])
        modulo_simple(ancho = 15);
    }
}








main();