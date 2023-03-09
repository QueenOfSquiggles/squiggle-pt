shader_type canvas_item;
render_mode blend_mix;

uniform vec4 colour : source_color = vec4(1.0);

void fragment() {
	vec4 sample = texture(TEXTURE, UV);
	COLOR = mix(colour, sample, sample);
	COLOR.a = sample.a;
	
}
