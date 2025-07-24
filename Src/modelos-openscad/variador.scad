
module rect_redondeado(radio_redondeo, dimensiones = [74, 124, 120]){
    hull(){
        translate([radio_redondeo,0,radio_redondeo])
        rotate([-90,0,0])
        cylinder(h=dimensiones[2], r=radio_redondeo, $fn=100);

        translate([dimensiones[0]-radio_redondeo,0,radio_redondeo])
        rotate([-90,0,0])
        cylinder(h=dimensiones[2], r=radio_redondeo, $fn=100);

        translate([dimensiones[0]-radio_redondeo,0,dimensiones[1]-radio_redondeo])
        rotate([-90,0,0])
        cylinder(h=dimensiones[2], r=radio_redondeo, $fn=100);

        translate([radio_redondeo,0,dimensiones[1]-radio_redondeo])
        rotate([-90,0,0])
        cylinder(h=dimensiones[2], r=radio_redondeo, $fn=100);
    }
}



module tapa(){
        rect_redondeado(radio_redondeo = 1, dimensiones = [71.5, 75, 2.5]);
        translate([0,1,10])
        rotate([90,0,0])
        scale([0.5,1,1])
        cylinder(r=6, h=2, center = true, $fn = 130);
    }

dimensiones = [74, 124, 120];
radio_redondeo = 3;

// cuerpo

    
// // selector rotatorio
// difference() {
//     translate([1.2,120,45.5])
//     hull(){
//         translate([dimensiones[0]/2,3,dimensiones[1]/3])
//         rotate([90,0,0])
//         cylinder(r = 11, h = 1, center = true, $fn = 100);
//         translate([dimensiones[0]/2,12,dimensiones[1]/3])
//         rotate([90,0,0])
//         cylinder(r = 10, h = 1, center = true, $fn = 100);
//     }
//     #translate([dimensiones[0]/2,16.5,dimensiones[1]/3])
//     scale([1,0.5,1])
//     sphere(r = 10, $fn = 100);
// }

// hueco esc
difference() {
    union(){
        //forntal
        translate([1.2,120,45.5])
        tapa();
        rect_redondeado(radio_redondeo = 3, dimensiones = [74, 124, 120]);
        translate([1.5,120,1.5]) 
        rect_redondeado(radio_redondeo = 2, dimensiones = [dimensiones[0]-radio_redondeo, dimensiones[1]-radio_redondeo, 2]);
        translate([dimensiones[0]/2-20,5,dimensiones[1]/3]){
            // hull() {
            //     translate([-3+1.2,118,45.5])
            //     rotate([90,0,0])
            //     cylinder(r=7, h=3, center = true, $fn = 100);
            //     translate([20+1.2,118,45.5])
            //     rotate([90,0,0])
            //     cylinder(r=12, h=3, center = true, $fn = 100);
            // }
            // hull(){
            //     translate([20+1.2,118,45.5])
            //     cube([55.5,3,1.5], center = true);
            //     translate([20+1.2,118,25.5])
            //     cube([53.5,3,1.5], center = true);
            //     translate([20+1.2,118,10.5])
            //     cube([48.5,3,1.5], center = true);
            // }
            // translate([-1,118.2, 31.5])
            // rotate([90,0,0])
            // cylinder(r=7, h=3.5, center = true, $fn = 100);

            // translate([2,118.2, 20.5])
            // rotate([90,0,0])
            // cylinder(r=7, h=3.5, center = true, $fn = 100);
        }
    }

    // translate([1.2,120,45.5]){
    //     translate([dimensiones[0]/2-20-3,5,dimensiones[1]/3])
    //     sphere(r = 6, $fn = 100);
    // }
}

