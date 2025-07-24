// D = 180
// d = 100
// dm = 125
//lmax = 210
h = 3;
r = 14;
r_prima = r*17/14;


module _tuerca(h = 1.5, r = 14, r_prima = 17, grosor = 12.5){
    difference() {
        union(){
            cylinder(h = h*4, r= r_prima, $fn = 300, center = true);
            translate([0,0,h])
            union(){
                cylinder(h = h*2.7, r= r_prima-r_prima*0.03, $fn = 300, center = true);
                cylinder(h = h*5.5, r= r_prima-r_prima*0.05, $fn = 300, center = true);
            }
            cylinder(h = h*2, r= r_prima*2-r_prima*sin(57), $fn = 6, center = true);
            translate([0,0,h*3.9])
            cylinder(h = h*0.3, r= r_prima-r_prima*0.03, $fn = 300, center = true);
        }
        cylinder(h = h*10, r = grosor, center = true, $fn = 300);
    }
}

module motor(){
    module _pata_delantera(){
        translate([115,0,0]){
        rotate([0,90,0])
        hull(){
            cylinder(r = 75, h = 6, $fn = 130, center = true);
            translate([80,0,0]) 
            cube([1, 75*2, 6], center = true);
        }
        rotate([0,90,0])
        translate([77.5,0,-17]) 
        cube([6, 75*2, 40], center = true);
        }
    }

    module _boquillas(){
         // Boquillas superiores
        translate([150,0,50]) 
        _tuerca(h = h, r = r, r_prima = r_prima, grosor = 12.5);
        translate([150,0,68]) 
        _tuerca(h = h, r = r, r_prima = r_prima, grosor = 12.5); 
    }

    module _pata_trasera(){
        //Pata trasera, la pequeña
        translate([20,0,0])
        rotate([0,180,0]){
            cylinder(r = 8, h = 80, $fn = 130, center = false);
            hull(){
                translate([0,0,70])
                cylinder(r = 10, h = 1, $fn = 130, center = false);
                translate([0,0,80])
                cylinder(r = 13, h = 1, $fn = 130, center = false);
            }
        }
    }

    module _cuerpo() {
        rotate([90,0,90]){
            translate([0,0,236]) 
            _tuerca(h = h, r = r, r_prima = r_prima, grosor = 12.5);
            rotate_extrude($fn = 130) 
            polygon(points = motor_puntos);

            for (i = [0:20:360]) {
                rotate([0,0,i])
                translate([0,45,8])
                cube([5, 12.5, 110]);
            }

            difference(){
                scale([1,1,1.2])
                sphere(r= 55, $fn = 130);
                union(){
                    translate([0,0,-70])
                    cylinder(r= 50, h = 50, $fn = 130, center = true);
                    cylinder(r= 100, h = 100, $fn = 130, center = false);
                }
            }

            translate([0,45,70]){
                cube([45, 60, 55], center = true);
                translate([10,23,0])
                rotate([0,90,0]){
                cylinder(r= 6, h = 50, $fn = 130, center = true);
                translate([0,0,25])
                cylinder(r= 7, h = 3, $fn = 130, center = true);}
                translate([0,15,0])
                cube([55, 2, 66], center = true);
            }
        }
    }

    _pata_delantera();
    _boquillas();
    _pata_trasera();
    _cuerpo();
}


motor_puntos = [[0,0], [55,0], [60,0], [60,3], [50,3], [50,5], [60,5], [60,6], [60,8], [50,8], [50,10], [50,105], [70,110], [70,120], [60,125],[58,125], [50,130], [50,170], [45,175], [45,180], [40,185], [20,200], [20,210], [23,210], [23, 216], [20,216], [22,216], [22, 220], [20, 225], [18,225], [18,230], [0, 230]];
motor();