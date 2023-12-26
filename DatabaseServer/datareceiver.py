import pymssql as mssql
from dto import convertRaw, convertSearch, convertNode, convertAlarm

class database:
    def __init__(self, name, ip, id, pw):
        self.db = name
        self.ip = ip
        self.username = id
        self.password = pw
        # self.db = "Hangwon4"
        # self.ip = '59.28.91.19:1433'
        # self.username='sa'
        # self.password='skf1234!'
        self.cursor = None

    def connect(self):
        conn = mssql.connect(server=self.ip, 
                             user=self.username, 
                             password=self.password, 
                             database=self.db)
        self.cursor = conn.cursor() # 쿼리 생성과 결과 조회를 위해 사용
        
    def nodes(self):
        result = []
        self.cursor.execute('''SELECT IDNode, IDParent, TreeType,
                                NodeType, NodeName, NodeStatus,
                                NodeActive 
                            FROM Node''')
        
        while True:
            row = self.cursor.fetchone() # 쿼리 결과의 다음 행을 가져와 리턴
            if row != None:
                result.append(row)
            else:
                return convertNode(result)
    
    def search(self, node_id: int, size: int = 50, index: int = 0):
        result = []
        self.cursor.execute('''SELECT IDMeasurement, IDNode, MeasDate,
                                    MeasValue, StartFreq, EndFreq,
                                    SampleRate, Speed, SpeedMin,
                                    SpeedMax, SpeedBegin, SpeedEnd, TimesignalLines, SpectraScaling, SpectraEUType
                                FROM Measurement
                                WHERE IDNode = {0}
                                ORDER BY (SELECT NULL), IDMeasurement DESC
                                OFFSET {1} ROWS FETCH NEXT {2} ROWS ONLY'''.format(node_id, index * size, size))
        
        while True:
            row = self.cursor.fetchone() # 쿼리 결과의 다음 행을 가져와 리턴
            if row != None:
                result.append(row)
            else:
                return convertSearch(result)
    
    def date(self, node_ids: [int], year: int, month: int, ny: int, nm: int):
        alarm = []
        meas = []
        i = list(map(lambda x: "IDNode = {0}".format(x), node_ids))
        i = " OR ".join(i)
        i = "({0})".format(i)

        self.cursor.execute('''SELECT 
                                    CONVERT(DATE, AlarmDate) AS AlarmDate,
                                    AlarmStatus
                                FROM 
                                    Alarm
                                WHERE 
                                    {0} AND
                                    '{1}-{2}-01' <= AlarmDate and AlarmDate < '{3}-{4}-1' 
                                GROUP BY 
                                    CONVERT(DATE, AlarmDate), AlarmStatus
                                ORDER BY
                                    AlarmDate;'''.format(i, year, month, ny, nm))
        while True:
            row = self.cursor.fetchone() # 쿼리 결과의 다음 행을 가져와 리턴
            if row != None:
                alarm.append({"date":row[0], "status":row[1]})
            else:
                break

        self.cursor.execute('''SELECT 
                                    CONVERT(DATE, MeasDate) AS MeasDate
                                FROM 
                                    Measurement
                                WHERE 
                                    {0} AND
                                    '{1}-{2}-01' <= MeasDate and MeasDate < '{3}-{4}-1' 
                                GROUP BY 
                                    CONVERT(DATE, MeasDate)
                                ORDER BY
                                    MeasDate;'''.format(i, year, month, ny, nm))
        
        while True:
            row = self.cursor.fetchone() # 쿼리 결과의 다음 행을 가져와 리턴
            if row != None:
                meas.append(row[0])
            else:
                return {"alarm": alarm, "meas": meas}

    def hour(self, node_ids: [int], year: int, month: int, day: int):
        alarm = []
        meas = []
        i = list(map(lambda x: "IDNode = {0}".format(x), node_ids))
        i = " OR ".join(i)
        i = "({0})".format(i)

        self.cursor.execute('''SELECT 
                                    IDNode,
                                    AlarmDate,
                                    AlarmStatus
                                FROM 
                                    Alarm
                                WHERE 
                                    {0} AND
                                    '{1}-{2}-{3}' = CONVERT(DATE, AlarmDate)
                                ORDER BY
                                    AlarmDate;'''.format(i, year, month, day))
        while True:
            row = self.cursor.fetchone() # 쿼리 결과의 다음 행을 가져와 리턴
            if row != None:
                alarm.append({"node_id":row[0], "date": row[1], "status": row[2]})
            else:
                break

        self.cursor.execute('''SELECT 
                                    IDNode,
                                    MeasValue,
                                    MeasDate
                                FROM 
                                    Measurement
                                WHERE 
                                    {0} AND
                                    '{1}-{2}-{3}' = CONVERT(DATE, MeasDate)
                                ORDER BY
                                    MeasDate;'''.format(i, year, month, day))
        
        while True:
            row = self.cursor.fetchone() # 쿼리 결과의 다음 행을 가져와 리턴
            if row != None:
                meas.append({"node_id":row[0], "meas_value": row[1], "meas_date": row[2]})
            else:
                return {"alarm": alarm, "meas": meas}

    def find(self, node_id: int, start: str, end:str):
        result = []

        self.cursor.execute('''SELECT IDMeasurement, IDNode, MeasDate,
                                    MeasValue, StartFreq, EndFreq,
                                    SampleRate, Speed, SpeedMin,
                                    SpeedMax, SpeedBegin, SpeedEnd, TimesignalLines, SpectraScaling, SpectraEUType
                                FROM Measurement
                                WHERE IDNode = {0}
                                And ('{1}' <= MeasDate and MeasDate <= '{2}')
                                ORDER BY (SELECT NULL), IDMeasurement DESC'''.format(node_id, start, end))
        
        while True:
            row = self.cursor.fetchone() # 쿼리 결과의 다음 행을 가져와 리턴
            if row != None:
                result.append(row)
            else:
                return convertSearch(result)
        

    def raw(self, measure_id):
        result = []
        self.cursor.execute('''SELECT * FROM MeasurementBinaryRaw WHERE IDMeasurement={0}'''.format(measure_id))
        
        while True:
            row = self.cursor.fetchone() # 쿼리 결과의 다음 행을 가져와 리턴
            if row != None:
                result.append(row)
            else:
                return convertRaw(result)
            
    def alarm(self, size: int = 50, offset: int = 0):
        result = []
        
        self.cursor.execute('''SELECT * FROM Alarm 
                                Order By IDAlarm DESC
                                OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY
                            '''.format(offset * size, size))
        
        while True:
            row = self.cursor.fetchone() # 쿼리 결과의 다음 행을 가져와 리턴
            if row != None:
                result.append(row)
            else:
                return convertAlarm(result)
            
    def search_alarm(self, node_id: [int], size: int = 50, offset: int = 0):
        result = []
        i = list(map(lambda x: "IDNode = {0}".format(x), node_id))
        i = " OR ".join(i)
        i = "({0})".format(i)

        self.cursor.execute('''SELECT * FROM Alarm 
                                WHERE {0}
                                Order By IDAlarm DESC
                                OFFSET {1} ROWS FETCH NEXT {2} ROWS ONLY
                            '''.format(i, offset * size, size))
        
        while True:
            row = self.cursor.fetchone() # 쿼리 결과의 다음 행을 가져와 리턴
            if row != None:
                result.append(row)
            else:
                return convertAlarm(result)
            
    def search_alarm_date(self, node_id: [int], start: str, end: str):
        result = []
        i = list(map(lambda x: "IDNode = {0}".format(x), node_id))
        i = " OR ".join(i)
        i = "({0})".format(i)
        q = '''SELECT TOP(200) * FROM Alarm 
                                WHERE {0} 
                                    And ('{1}' <= AlarmDate and AlarmDate <= '{2}')
                                Order By IDAlarm DESC
                            '''.format(i, start,end)
        print(q)
        self.cursor.execute(q)
        
        while True:
            row = self.cursor.fetchone() # 쿼리 결과의 다음 행을 가져와 리턴
            if row != None:
                result.append(row)
            else:
                return convertAlarm(result)
            
