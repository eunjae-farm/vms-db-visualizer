import pymssql as mssql
from dto import convertRaw, convertSearch, convertNode

class database:
    def __init__(self):
        self.db = "Hangwon4"
        self.ip = '59.28.91.19:1433'
        self.username='sa'
        self.password='skf1234!'
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
            
    def raw(self, measure_id):
        result = []
        self.cursor.execute('''SELECT * FROM MeasurementBinaryRaw WHERE IDMeasurement={0}'''.format(measure_id))
        
        while True:
            row = self.cursor.fetchone() # 쿼리 결과의 다음 행을 가져와 리턴
            if row != None:
                result.append(row)
            else:
                return convertRaw(result)