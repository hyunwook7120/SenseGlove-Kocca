import time
import os
import serial
import tkinter as tk

from tkinter import messagebox

arduino = serial.Serial(port='COM20', baudrate=9600, timeout=1)

def set_setpoint(setpoint):
    """
    아두이노에 새로운 Setpoint 값을 전달하고 현재 온도 반환
    """
    arduino.write(f"{setpoint}\n".encode())
    
    while True:
        line = arduino.readline().decode('utf-8').strip()
        if line:
            print(f"Arduino: {line}")
            try:
                temp_part = line.split('Temperature: ')[1].split('C')[0].strip()
                current_temp = float(temp_part)
                return current_temp
            except (ValueError, IndexError) as e:
                print(f"Error parsing temperature: {e}")
                return None
        break
        
        
def show_popup_message(message):
    """
    목표 온도에 도달했을 때 나타나는 팝업 메시지 창
    """
    root = tk.Tk()
    root.withdraw()  # 메인 윈도우 숨김
    messagebox.showinfo("Goal Achieved", message)
    root.destroy()  # 팝업 창을 닫으면 루트를 닫음

def read_file_continuously(filename):
    try:
        while True:
            with open(filename, "r") as file:
                data = file.read().strip()
                print(f"File Content:\n{data}\n")
                
                # 파일에서 읽은 값을 set_setpoint에 전달 (숫자로 변환해서 전달)
                try:
                    setpoint = float(data)  # 파일 내용이 온도 값일 경우, 숫자로 변환
                    current_temp = set_setpoint(setpoint)  # set_setpoint 함수 호출
                    if current_temp is not None:
                        print(f"Setpoint {setpoint}C applied. Current temperature: {current_temp}C")
                        
                        # 목표 온도에 도달했는지 확인
                        if abs(current_temp - setpoint) < 0.5:  # 0.5도 이내면 목표 온도 도달
                            show_popup_message(f"Target temperature {setpoint}C reached!")
                    else:
                        print(f"Setpoint {setpoint}C applied. Current temperature: Not available")
                except ValueError:
                    print("Invalid data in file, cannot convert to setpoint.")
            
            # 1초 대기
            time.sleep(1)
    
    except KeyboardInterrupt:
        print("Program interrupted and stopped.")


script_dir = os.path.dirname(os.path.abspath(__file__))  # 스크립트의 절대 경로
filepath = os.path.join(script_dir, 'Assets', 'Resources', 'temperature.txt')
read_file_continuously(filepath)
