shader_type canvas_item;

uniform vec4 flash_color : source_color;
uniform bool is_white = false;

// Called for every pixel the material is visible on.
void fragment() {
	vec4 color = texture(TEXTURE, UV);
	color.r = flash_color.r;
	color.g = flash_color.g;
	color.b = flash_color.b;
	color.a = color.a * flash_color.a;
	
	if (is_white)
	{
		COLOR = color;
	}
}

//void light() {
//	// Called for every pixel for every light affecting the CanvasItem.
//	// Uncomment to replace the default light processing function with this one.
//}
