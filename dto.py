import numpy as np
import struct

def __convertData(row):
    result = []
    width = 2

    for idx in range(0, len(row), width):
        raw = row[idx:idx+width]
        # if r == True:
            # data = float(struct.unpack('>h', raw)[0])
        # else:
        data = float(struct.unpack('<h', raw)[0])
            
        result.append(data)
    return np.array(result)

def convertNode(data):
    return list(map(lambda x: {"node":x[0], "parent": x[1], "treetype": x[2],
                               "nodetype":x[3], "name":x[4].strip(), "status":x[5],
                               "active":x[6]}, data))

def convertSearch(data): # SpectraScaling, SpectraEUType
    return list(map(lambda x: {"id":x[0], "node": x[1], "date": x[2],
                               "value":x[3], "start_freq":x[4], "end_freq":x[5],
                               "sample_rate":x[6],"speed":x[7],"speed_min":x[8],
                               "speed_max":x[9],"speed_begin":x[10],"speed_end":x[11], 'time_signal_lines': x[12], 'SpectraScaling': x[13], 'SpectraEUType':x[14]}, data))

def convertRaw(data):
    return list(map(lambda x: {"id": x[0], "data_type": x[1], "rawdata_type": x[2],
                               "scale_factor":x[3], "rawdata": __convertData(x[4])}, data))

