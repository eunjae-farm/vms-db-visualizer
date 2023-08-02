import pymssql as mssql
import numpy as np
import struct

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
                                SortOrderId, NodeActive 
                            FROM Node''')
        
        while True:
            row = self.cursor.fetchone() # 쿼리 결과의 다음 행을 가져와 리턴
            if row != None:
                result.append(row)
            else:
                return result
            