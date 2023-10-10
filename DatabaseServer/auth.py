#%%
import hashlib as hash
import random as rd

#%%
class Authorization:
    def __init__(self):
        self.hash = {}
        
    def __salt(self, length: int) -> str:
        r = ""
        t = "1234567890qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLMNBVCXZ"
        for _ in range(length):
            idx = rd.randint(0, len(t) - 1)
            r += t[idx]
        return r
        
    def login(self, id: str, pw: str, dbName: str, dbIp: str) -> str:
        text = "{0}_{1}_{2}".format(id, pw, self.__salt(0)).encode()
        h = hash.sha512(text).hexdigest()
        self.hash[h] = {"id": id, "pw": pw, "name":dbName, "ip": dbIp}
        return h
        
    def valid(self, h: str) -> bool:
        if h in self.hash:
            return True
        else:
            return False
        
    def get(self, token: str):
        return self.hash[token]
        
# %%
