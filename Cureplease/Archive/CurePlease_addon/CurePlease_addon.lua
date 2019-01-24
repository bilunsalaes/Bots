_addon.name = 'CurePlease_addon'
_addon.author = 'Daniel_H'
_addon.version = '1.0'
_addon_description = ''
_addon.commands = {'cureplease', 'cp'}

-- Some of this information was borrowed from code from: Kenshi, Copyright © 2016
-- UDP connection thanks to several online tutorials

local socket = require("socket")

local port = 19769
local ip = "127.0.0.1"

if windower then
    packets = require("packets")

    windower.register_event('addon command', function(input, ...)
    local cmd = string.lower(input)
    local args = {...}
        if cmd == "settings" then 
            if args[1] and args[2] then
                ip = args[1]
                port = args[2]
            end
        end

        if cmd == "check" then
            windower.add_to_chat(207, "Current CurePlease info: " .. "IP address: " .. ip .. " / Port number: " .. port)
        end
    end)


end

if ashita then

    require 'common'

ashita.register_event('command', function(command, ntype)
    -- Get the arguments of the command..
    local args = command:args();
    if (args[1]:lower() ~= '/cureplease' and args[1]:lower() ~= "/cp") then
        return false;
    end

    if (#args >= 4 and args[2] == 'settings') then
        ip = args[3]
        port = args[4]
    end 

    if (#args == 2 and args[2] == 'check') then
        print("Current CurePlease info: " .. "IP address: " .. ip .. " / Port number: " .. port)
    end
    return true;
end);

end

local CharacterBuffData = {}

function grabName(userIndex) 
    if windower then
        if userIndex ~= 9 then
            if windower.ffxi.get_mob_by_id(userIndex) == nil then 
                return "NONE"
            else
                found_character = windower.ffxi.get_mob_by_id(userIndex).name
                return found_character
            end
        else
            return "NONE"
        end
    end
    if ashita then
        if userIndex ~= 9 then
            return AshitaCore:GetDataManager():GetEntity():GetName(userIndex)
        else
            return "NONE"
        end
    end
end

function send_required_string(DaTa)


local CP_connect = assert(socket.udp())
      CP_connect:settimeout(1)

    assert(CP_connect:sendto(DaTa, ip, port))
    
    CP_connect:close()
end

function convert_to_data(id, data) 
    if id == 0x076 then
        for k = 0, 4 do
            if ashita then 
                local Uid = struct.unpack('H', data, 8+1 + (k * 0x30))
                if Uid ~= 0 and Uid ~= nil then
                    userIndex = Uid
                else
                    userIndex = 9
                end
            end
            if windower then
                local Uid = data:unpack('I', k*48+5)
                if Uid ~= 0 and Uid ~= nil then
                    userIndex = Uid
                else
                    userIndex = 9
                end
            end
            
            -- FOR EACH MEMBER REMOVE PREVIOUS CHARACTERS DATA
            Buffs = ""
            member_Name = ""

            -- GRAB THE MEMBERS NAME
            member_Name = grabName(userIndex)

            -- RUN THROUGH A LOOP TO GET MEMBER BUFFS
            for i = 1, 32 do
                current_buff = data:byte(k*48+5+16+i-1) + 256*( math.floor( data:byte(k*48+5+8+ math.floor((i-1)/4)) / 4^((i-1)%4) )%4)
                if current_buff ~= 255 and current_buff ~= 0 then
                    Buffs=Buffs..current_buff..","
                end
            end

           if member_Name ~= nil then
                processed_data = member_Name.."-"..Buffs

                if (member_Name ~= "NONE") then
                    send_required_string(member_Name.."-"..Buffs)
                end   
           end 
        end   
    end
end

if windower then

-- BEGIN WINDOWER CODE ---------------------------------------------------------------------------------
    windower.register_event('incoming chunk', function (id, data)
        convert_to_data(id, data)
    end)

-- END WINDOWER CODE -----------------------------------------------------------------------------------

end
if ashita then
-- BEGIN ASHITA CODE -----------------------------------------------------------------------------------

    ashita.register_event('incoming_packet', function(id, size, packet)

    if id == 0xB then
        zoning_bool = true
    elseif id == 0xA and zoning_bool then
        zoning_bool = false
    end

        if zoning_bool then 

        else
            convert_to_data(id, packet)
        end 
        return false;
    end);

-- END ASHITA CODE -------------------------------------------------------------------------------------
end