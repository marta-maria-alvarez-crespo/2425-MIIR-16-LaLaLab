// Conector caja de la planta 

r = 14;
ancho = 20;
radio_eje = 6;

module _seta_cuerpo(r = 14){
    cylinder(r = r*1.75, h = 0.1, $fn = 130);
    cylinder(r = r, h = r/3, $fn = 130);
    difference() {
        translate([0,0,r/3])
        cylinder(r = r*1.11, h = r/3, $fn = 6);

        difference() {
            translate([0,0,r/3-1])
            cylinder(r = r*2, h = r/3+2, $fn = 130);
            translate([0,0,r/3-1])
            cylinder(r = r, h = r/3+2, $fn = 130);
        }
    }    
    translate([0,0,r/1.5])
    cylinder(h = r/1.5, r = r/1.2, $fn = 130);
}                                                                                                                                                                                       

module _seta_boton(r = 14){
    translate([0,0,r])
    cylinder(r = r/1.12, h = r/3, $fn = 130);
    hull(){
        translate([0,0,r*1.3])
        cylinder(r = r/1.12, h = r/2, $fn = 130);
        translate([0,0,r*1.1+r/3])
        cylinder(r = r*1.1, h = r/9, $fn = 130);
    }
}

module seta(r = 14){
    _seta_cuerpo(r = r);
    _seta_boton(r = r);
}

module modulo_simple(ancho = 25){
    perfil = [[0,0], [49.7, 0], [49.7, 18], [65,18], [65,63], [49.7, 63],[49.7, 81],[0,81],[0,63], [10,63], [10,18], [0,18]];
    rotate([90,0,0])
    mirror([0,0,1])
    linear_extrude(height = ancho) 
    polygon(points = perfil);
}

module modulo_interruptor(ancho = 25, radio_eje = 6){
    altura = 81;
    modulo_simple(ancho = ancho);
    translate([65,2.5,altura/2.3])
    rotate([-90,0,0])
    union(){
        cylinder(h = ancho-5, r = 3, $fn = 130);
        difference(){
            union(){
                scale([1,1,0.2])
                sphere(r = radio_eje, $fn = 130);
                translate([0,0,ancho-5])
                mirror([0,0,1])
                scale([1,1,0.2])
                sphere(r = radio_eje, $fn = 130);
            } cylinder(h = ancho-5, r = radio_eje, $fn = 130);
        } 
    }
}

module palanca_interruptor(ancho = 25, radio_eje = 6){
    altura = 81;
    cylinder(h = ancho-5, r = radio_eje, $fn = 130, center = true);
    translate([0,-1.5,-ancho/2+2.5]) 
    cube([10, 3, ancho-5], center = false);
    hull(){
        translate([13,0,0]) 
        cylinder(h = ancho-4.5, r = radio_eje/2, $fn = 130, center = true);
        translate([10,0,0]) 
        cylinder(h = ancho-5, r = radio_eje/3, $fn = 130, center = true);
    }
}

module modulo_completo(ancho = ancho, radio_eje = radio_eje, simple = false){
    if (simple) {
        color("white")
        modulo_interruptor(ancho = ancho, radio_eje = radio_eje);
        color("black")
        translate([65,ancho/2,81/2.3])
        rotate([-90,0,0])
        palanca_interruptor(ancho = ancho, radio_eje = radio_eje);

    } else {
        for (i = [0:1.3:2]) {
            color("black")
            translate([65,ancho/2*i+ancho/2,81/2.3])
            rotate([-90,0,0])
            palanca_interruptor(ancho = ancho, radio_eje = radio_eje);
            color("white")
            translate([0,ancho/2*i,0])
            modulo_interruptor(ancho = ancho, radio_eje = radio_eje);
        }
    }
}

module rack(l = 40){
    difference(){
        cube([30, 5, l], center = true);
        union(){
            translate([0,2,0]) 
            cube([25, 5, l+1], center = true);
            translate([0,0,0]) 
            cube([28, 3, l+1], center = true);
}
}}


module sensor_nivel(){
    color("gray")
    hull(){
        translate([400,400,0])
        cylinder(r = 10, h = 2, center = true);
        difference(){
            translate([440,400,0])
            cube([30,30,2], center = true);
            translate([450,400,0])
            cube([30,30,2], center = true);
        }
    }
    translate([435,400,14]){
        color("gray")
        cube([2,30,30], center = true);}


    // sensor nivel
    translate([435,400,14]){
        rotate([0,90,0])
        color("black")
        cylinder(r = 10, h = 10, center = true);
        translate([40,0,0])
        rotate([0,90,0])
        color("black")
        cylinder(r = 6, h = 30, center = true);
        color("yellow")
        difference() {
            translate([12,0,0])
            rotate([0,90,0])
            cylinder(r = 8, h = 50, center = true);
            translate([-13,0,0])
            rotate([0,90,0])
            color("black")
            cylinder(r = 7, h = 5, center = true);
        }

        translate([-13,0,0])
        rotate([0,90,0])
        color("black", alpha=0.5)
        cylinder(r = 6.9, h = 2, center = true);

        translate([45,0,0])
        rotate([0,90,0])
        color("black")
        cylinder(r = 4, h = 30, center = true);
    }

}


module sensor_on_off(){
    
}
// palanca_interruptor(ancho = ancho, radio_eje = radio_eje);
// modulo_interruptor(ancho = ancho, radio_eje = radio_eje);
// modulo_completo(ancho = ancho, radio_eje = radio_eje, simple = false);
// modulo_completo(ancho = ancho, radio_eje = radio_eje, simple = true);
// rack(l = 40);
//  _seta_cuerpo(r = r);
// _seta_boton(r = r);
