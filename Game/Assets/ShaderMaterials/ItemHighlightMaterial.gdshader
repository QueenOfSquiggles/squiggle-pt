shader_type spatial;
render_mode unshaded, depth_draw_never;

uniform vec4 shine_color : source_color = vec4( 1.0, 1.0, 1.0, 1.0 );
uniform float shine_speed : hint_range(0.01, 5.0) = 3.0;
uniform float shine_width : hint_range(1.0, 100.0) = 3.0;
uniform float vertical_scale : hint_range(0.1, 5.0) = 2.0;



float fresnel(float amount, vec3 normal, vec3 view)
{
	return pow((1.0 - clamp(dot(normalize(normal), normalize(view)), 0.0, 1.0 )), amount);
}


void fragment( )
{
	vec3 vertex = VERTEX;
	float frequency = sin((vertex.z * vertical_scale) + TIME * shine_speed) * shine_width;
	float fres = fresnel(2.0, NORMAL, VIEW);
	ALBEDO = shine_color.rgb;
	ALPHA = clamp(frequency * fres, 0.0, 1.0);

}