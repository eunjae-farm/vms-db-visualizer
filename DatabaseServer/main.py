from flask import Flask, jsonify, request
from datareceiver import database
from auth import Authorization
from convert import convert_spectra 
import numpy as np

author = Authorization()
app = Flask(__name__)

@app.route('/node')
def get_node():
    params = request.get_json()
    if not author.valid(params['token']):
        return jsonify({"error": "token is not matching from database"})
    
    db = author.get(params['token'])
    data = database(db['name'], db['ip'], db['id'], db['pw'])
    data.connect()
    node = data.nodes()    
    return jsonify(node)

@app.route('/search')
def get_search():
    params = request.get_json()
    if not author.valid(params['token']):
        return jsonify({"error": "token is not matching from database"})
    
    db = author.get(params['token'])
    data = database(db['name'], db['ip'], db['id'], db['pw'])
    data.connect()
    search = data.search(params['id'], params['size'], params['offset'])
    return jsonify(search)

@app.route('/raw')
def get_raw():
    params = request.get_json()
    if not author.valid(params['token']):
        return jsonify({"error": "token is not matching from database"})
    
    db = author.get(params['token'])
    data = database(db['name'], db['ip'], db['id'], db['pw'])
    data.connect()
    search = data.raw(params['id'])
    return jsonify(search)

@app.route('/fft')
def get_fft():
    params = request.get_json()
    if not author.valid(params['token']):
        return jsonify({"error": "token is not matching from database"})
    
    db = author.get(params['token'])
    data = database(db['name'], db['ip'], db['id'], db['pw'])
    data.connect()
    search = data.raw(params['id'])
    item = list(filter(lambda x: x['data_type'] == 0, search))
    if len(item) == 1: 
        item = item[0]
        x, y = convert_spectra(params['timeline'], params['freq'], item)
        return jsonify({"freq": x, "itensitiy": y})
    else:
        return jsonify({"error": "this log data has not stored charts data"})
    

@app.route('/charts')
def get_charts():
    params = request.get_json()
    if not author.valid(params['token']):
        return jsonify({"error": "token is not matching from database"})
    
    db = author.get(params['token'])
    data = database(db['name'], db['ip'], db['id'], db['pw'])
    data.connect()
    raw = data.raw(params['id'])
    item = list(filter(lambda x: x['data_type'] == 2, raw))
    if len(item) == 1: 
        item = item[0]
        return jsonify({"duration": len(item['rawdata']) / params['sample_rate'], "data": (np.array(item['rawdata']) * item['scale_factor']).tolist()})
    else:
        return jsonify({"error": "this log data has not stored charts data"})

 
@app.route('/alarm')
def get_alarm():
    params = request.get_json()
    if not author.valid(params['token']):
        return jsonify({"error": "token is not matching from database"})
    
    db = author.get(params['token'])
    data = database(db['name'], db['ip'], db['id'], db['pw'])
    data.connect()
    
    if 'node' in params:
        search = data.search_alarm(params['node'], 
                                   params['size'], 
                                   params['offset'])
    else:
        search = data.alarm(params['size'], 
                            params['offset'])
    
    return jsonify(search)


@app.route('/login', methods=["POST"])
def login():
    params = request.get_json()
    token = author.login(params['id'], params['pw'], params['name'], params['ip'])
    return jsonify({"token": token})

@app.route('/logout', methods=["POST"])
def logout():
    params = request.get_json()
    r = author.logout(params['token'])
    return jsonify({"result": r})
    
    
if __name__ == "__main__":
    app.run(port=5001, host="0.0.0.0")
    # db = database()
    # db.connect()
    # result = db.nodes()
    # print(result)
    # print()
    # print()
    # print()
    # print(db.search(6))
    # print()
    # print()
    # print()
    # print(db.raw(778096))


