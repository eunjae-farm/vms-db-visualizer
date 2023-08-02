from datareceiver import database

if __name__ == "__main__":
    db = database()
    db.connect()
    result = db.nodes()
    print(result)