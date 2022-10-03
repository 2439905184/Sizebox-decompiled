-- to save a global variable than can be used between multiple scripts, use globals
-- it is a global table so you can save data and share it with other scripts
-- but beware because you can end up using some used in other script by other person

-- so use a naming convention to make sure that you are only using your own variables
-- i keep this a "n" just as example

-- there are two scripts, globalTable1.lua and globalTable2.lua, basically there are 
-- counters, and are using the same variable

-- if you press F12 in game, you should see a console command and there two scripts
-- must be counting the same value "n"

-- you can also save a table inside, or any data that you may need, just remenber to
-- update the global table if you modify your values

function Update () 
    if globals["n"] == nil then
        globals["n"] = 0
    end
    local n = globals["n"]
    log("Counter 1: " .. n)
    globals["n"] = n + 1
end
