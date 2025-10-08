shader_type canvas_item;

// Called for every vertex the material is visible on.
void vertex() {

}

// Called for every pixel the material is visible on.
void fragment() {
	vec4 color = texture(TEXTURE, UV);
	color.r = 0.5;
	color.b = 0.5;
	color.g = 0.5;
	//color.a = 0.5;
	//COLOR = color;
}

//void light() {
//	// Called for every pixel for every light affecting the CanvasItem.
//	// Uncomment to replace the default light processing function with this one.
//}
