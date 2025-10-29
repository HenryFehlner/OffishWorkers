shader_type canvas_item;

uniform vec4 flash_color : source_color;
uniform bool is_white = false;

// Called for every pixel the material is visible on.
void fragment() {
	vec4 tex_color = texture(TEXTURE, UV);

	if (is_white)
	{
		// Apply the flash color and use mix to apply the alpha, gives control of the color intensity
		COLOR = mix(tex_color, vec4(flash_color.rgb, tex_color.a), flash_color.a);
	}
	else
	{
		COLOR = tex_color;
	}
}

//void light() {
//	// Called for every pixel for every light affecting the CanvasItem.
//	// Uncomment to replace the default light processing function with this one.
//}
