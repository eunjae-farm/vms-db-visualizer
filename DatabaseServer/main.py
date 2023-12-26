from flask import Flask, jsonify, request
from datareceiver import database
from auth import Authorization
from convert import convert_spectra 
import numpy as np
import datetime

__VERSION__ = '0.1.5'

author = Authorization()
app = Flask(__name__)

@app.route('/node', methods=["POST"])
def get_node():
    params = request.get_json()
    if not author.valid(params['token']):
        return jsonify({"error": "token is not matching from database"})
    
    db = author.get(params['token'])
    data = database(db['name'], db['ip'], db['id'], db['pw'])
    data.connect()
    node = data.nodes()    
    return jsonify(node)


# 1달 단위로, 데이터가 있는 정보를 추출하여 반환함.
# Need to Search item from Node Id
@app.route('/month', methods=["POST"])
def month():
    params = request.get_json()
    if not author.valid(params['token']):
        return jsonify({"error": "token is not matching from database"})
    db = author.get(params['token'])
    data = database(db['name'], db['ip'], db['id'], db['pw'])
    data.connect()
    result = data.date(params["node"], params["year"], params["month"], params["ny"], params["nm"])
    return jsonify(result)

# 1달 단위로, 24시간 기준으로 데이터가 있는 정보를 추출하여 반환함.
@app.route('/date', methods=["POST"])
def date():
    params = request.get_json()
    if not author.valid(params['token']):
        return jsonify({"error": "token is not matching from database"})
    db = author.get(params['token'])
    data = database(db['name'], db['ip'], db['id'], db['pw'])
    data.connect()
    result = data.hour(params["node"], params["year"], params["month"], params["day"])
    return jsonify(result)

# 선택된 시간을 기준으로 아래의 /search API와 동일하게 값을 반환시킴.
# 단, 시간 + 범위 정보가 넘어와야함.
# 그 이유로는 센서 데이터는 동시에 입력이 되는 것이 아니므로 ><
@app.route('/find_search', methods=["POST"])
def find_date():
    params = request.get_json()
    if not author.valid(params['token']):
        return jsonify({"error": "token is not matching from database"})
    db = author.get(params['token'])
    data = database(db['name'], db['ip'], db['id'], db['pw'])
    data.connect()
    result = data.find(params["node"], 
                       params["start"], 
                       params["end"])
    
    return jsonify(result)

@app.route('/search', methods=["POST"])
def get_search():
    params = request.get_json()
    if not author.valid(params['token']):
        return jsonify({"error": "token is not matching from database"})
    
    db = author.get(params['token'])
    data = database(db['name'], db['ip'], db['id'], db['pw'])
    data.connect()
    search = data.search(params['id'], params['size'], params['offset'])
    return jsonify(search)

@app.route('/raw', methods=["POST"])
def get_raw():
    params = request.get_json()
    if not author.valid(params['token']):
        return jsonify({"error": "token is not matching from database"})
    
    db = author.get(params['token'])
    data = database(db['name'], db['ip'], db['id'], db['pw'])
    data.connect()
    search = data.raw(params['id'])
    return jsonify(search)

@app.route('/fft', methods=["POST"])
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

@app.route('/charts', methods=["POST"])
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

 
@app.route('/alarm', methods=["POST"])
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

@app.route("/alarm_date", methods=["POST"])
def get_alarm_date():
    params = request.get_json()
    if not author.valid(params['token']):
        return jsonify({"error": "token is not matching from database"})
    db = author.get(params['token'])
    data = database(db['name'], db['ip'], db['id'], db['pw'])
    data.connect()
    search = data.search_alarm_date(params['node'], 
                                params['start'], 
                                params['end'])
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

@app.route("/healthy", methods=["GET"])
def healthyCheck():
    return jsonify({"time": datetime.datetime.now().isoformat(), "version": __VERSION__})

if __name__ == "__main__":
    app.run(port=5001, host="0.0.0.0")
