local a = Vector3:new(1,2,3)
local b = Vector3:new(4,5,6)
local c = a + b
Log(c)
Log(c.x .. " " .. c.y .. " " .. c.z)
local d = c * 10
Log(d + c / 10 - a)
Log(-d)
local e = Vector3:new(1,2,3)
if a == e then
    Log("A Equals E")
end

if a != b then
    Log("A is different than B")
end

f = a
a = a * 2
Log(f .. " " .. a)


local current = Vector3.zero
function Update()
    current = Vector3.Lerp(current, Vector3.right * 1000, time.delta_time * 0.01)
    Log(player.transform.eulerAngles)
end