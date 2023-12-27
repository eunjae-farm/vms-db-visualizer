@echo off

pip3 install -r requirements.txt
pip3 install pyinstaller
python3 -m PyInstaller -F --console main.py
