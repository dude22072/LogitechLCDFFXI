_addon.name = 'Logitech GamePanel'
_addon.author = 'dude2072'
_addon.version = '0.1.0'
_addon.commands = {'LGP', 'lcd', 'LogitechGamePanel'}

packets = require('packets')

local host, port = "127.0.0.1", 33941
local socket = require("socket")
local tcp = assert(socket.tcp())
connected = false
local timer = 0
local timerthreshold = 60
expready=false
--jobid = {"WAR","MNK","WHM","BLM","RDM","THF","PLD","DRK","BST","BRD","RNG","SAM","NIN","DRG","SMN","BLU","COR","PUP","DNC","SCH","GEO","RUN"}

approved_commands = {
    'connect','c',
    'disconnect','d',
    'reconnect','r',
    'help','h'
}

xp = {
        registry = {},
        total = 0,
        rate = 0,
        current = 0,
        tnl = 0
    }
    
prevxp = {
        registry = {},
        total = 0,
        rate = 0,
        current = 0,
        tnl = 0
    }

while connected do
    timer = timer + 1
    windower.add_to_chat(8,"connected")
    if timer >= timerthreshold then
        mainLoop()
        timer = 0
    end
end

windower.register_event('login',function()
    initialize()
end)

windower.register_event('addon command',function(command, ...)
    windower.add_to_chat(8,command)
    local commands = {...}
        if command == 'connect' or command == 'c' then
            connect()
        elseif command == 'disconnect' or command == 'd' then
            disconnect()
        elseif command == 'reconnect' or command == 'r' then
            reconnect()
        elseif command == 'help' or command == 'h' then
            display_help()
        elseif command == 'debug' then
            tcp:send(...)
        end
end)

windower.register_event('chat message', function(message, sender, mode, GM)
    if mode == 3 then
        sendTell(sender, message)
    end
end)

windower.register_event('unload', function()
    disconnect()
end)

function mainLoop()
    --local player = windower.ffxi.get_player()
    --windower.add_to_chat(8,player.name)
end

function display_help()
    windower.add_to_chat(8,_addon.name..' v'.._addon.version..': Command Listing')
    windower.add_to_chat(8,'   (c)onnect - Attempts to connect to LCD Applet')
    windower.add_to_chat(8,'   (d)isconnect - Disconnects from LCD Applet')
    windower.add_to_chat(8,'   (r)econnect - runs disconnect then connect')
end

windower.register_event('incoming chunk',function(id,org,modi,is_injected,is_blocked)
    if is_injected then return end
    if true then
        local packet_table = packets.parse('incoming', org)
        if id == 0x2D then
            exp_msg(packet_table['Param 1'],packet_table['Message'])
        elseif id == 0x61 then
            xp.current = packet_table['Current EXP']
            xp.total = packet_table['Required EXP']
            xp.tnl = xp.total - xp.current
            chunk_update = true
        end
    end
    
    if connected and expready then
        if xp.current ~= prevxp.current or xp.total ~= prevxp.total then
            tcp:send("\nexp:"..xp.current.."\nexpn:"..xp.total)
            prevxp.current = xp.current
            prevxp.total = xp.total
        end
    end
end)

windower.register_event('job change',function(main_job_id, main_job_level, sub_job_id, sub_job_level)
     tcp:send("job:"..jobid[main_job_id].."\njobl:"..main_job_level.."\nsjob:"..jobid[sub_job_id].."\nsjobl:"..sub_job_level.."\n")
end)

windower.register_event('weather change',function(wid)
     tcp:send("wth:"..wid.."\n")
end)

windower.register_event('hp change',function(nhp,ohp)
     tcp:send("hp:"..nhp.."\n")
end)

windower.register_event('hpmax change',function(nhp,ohp)
     tcp:send("mhp:"..nhp.."\n")
end)

windower.register_event('mp change',function(nmp,omp)
     tcp:send("mp:"..nmp.."\n")
end)

windower.register_event('mpmax change',function(nmp,omp)
     tcp:send("mmp:"..nmp.."\n")
end)

windower.register_event('time change',function(new,old)
     if(connected) then
        local pos = windower.ffxi.get_position()
        tcp:send("tim:"..new.."\npos:"..pos.."\n")
     end
end)

windower.register_event('day change',function(new,old)
     tcp:send("day:"..new.."\n")
end)

windower.register_event('moon change',function(new,old)
     --tcp:send("tim:"..new.."\n")
end)

windower.register_event('zone change',function(new,old)
     tcp:send("loc:"..new.."\n")
end)

function exp_msg(val,msg)
    local t = os.clock()
    if msg == 8 or msg == 105 then
        xp.registry[t] = (xp.registry[t] or 0) + val
        xp.current = math.min(xp.current + val,55999)
        if xp.current > xp.tnl then
            xp.current = xp.current - xp.tnl
        end
        chunk_update = true
    end
end

function connect()
    tcp:connect(host,port)
    windower.add_to_chat(8,"connect")
    connected = true
    local player = windower.ffxi.get_player()
    local gameInfo = windower.ffxi.get_info()
    local pos = windower.ffxi.get_position()
    local cords = windower.ffxi.get_map_data(player.id)
    if player.sub_job_level ~= nil then
        tcp:send("name:"..player.name.."\njob:"..player.main_job.."\njobl:"..player.main_job_level.."\nsjob:"..player.sub_job.."\nsjobl:"..player.sub_job_level.."\nhp:"..player.vitals.hp.."\nmhp:"..player.vitals.max_hp.."\nmp:"..player.vitals.mp.."\nmmp:"..player.vitals.max_mp.."\nexp:"..xp.current.."\nexpn:"..xp.total.."\nloc:".."?".."\nlcd".."?".."\nncd".."0".."\ntim:"..gameInfo.time.."\nDAY:"..gameInfo.day.."\n"--[[X:"..pos.x.."\nY:"..pos.y.."\nZ:"..pos.z.."\n]].."deg:".."0".."\nwth:"..gameInfo.weather.."\npos:"..pos)
    else
        tcp:send("name:"..player.name.."\n".."job:"..player.main_job.."\n".."jobl:"..player.main_job_level.."\nsjob:---\nsjobl:00\nhp:"..player.vitals.hp.."\nmhp:"..player.vitals.max_hp.."\nmp:"..player.vitals.mp.."\nmmp:"..player.vitals.max_mp.."\nexp:"..xp.current.."\nexpn:"..xp.total.."\nloc:".."?".."\nlcd".."?".."\nncd".."0".."\ntim:"..gameInfo.time.."\nDAY:"..gameInfo.day.."\n"--[[X:"..pos.x.."\nY:"..pos.y.."\nZ:"..pos.z.."\n]].."deg:".."0".."\nwth:"..gameInfo.weather.."\npos:"..pos)
    end
    expready = true
end

function disconnect()
    tcp:close()
    windower.add_to_chat(8,"disconnect")
    connected = false
    timer = 0
end

function reconnect()
    disconnect()
    connect()
end

function sendTell(sender, message)
    tcp:send("tell:"..sender..":"..message.."\n")
end