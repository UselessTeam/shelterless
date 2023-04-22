shader_type canvas_item;

uniform sampler2D noise: filter_nearest, repeat_enable;

void fragment() {
	vec2 uv = vec2(UV.x + TIME*0.15, UV.y);
	COLOR.rgba = texture(noise, uv).rgba;
}
