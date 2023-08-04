from datareceiver import database

if __name__ == "__main__":
    db = database()
    db.connect()
    result = db.nodes()
    print(result)
    print()
    print()
    print()
    print(db.search(6))
    print()
    print()
    print()
    print(db.raw(778096))


