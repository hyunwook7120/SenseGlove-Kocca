import tkinter as tk
from tkinter import messagebox
import csv
import os
import time

name = ""

data_list = []

# 최대 입력 횟수
max_entries = 120

# 현재 수정 중인 인덱스
current_index = None

# 현재 입력 횟수
current_entry_count = 0

# MaxForce 값을 매핑하는 딕셔너리
force_map = {
    1: 0.0,
    2: 0.1,
    3: 0.3,
    4: 0.6,
    5: 1.0
}

def save_data():
    global data_list
    filename = f"{name}.csv"
    
    file_exists = os.path.isfile(filename)
    
    with open(filename, mode='a', newline='') as file:
        writer = csv.writer(file)
        if not file_exists:
            writer.writerow(['name', 'perception', 'maxforce', 'timestamp'])
        writer.writerows(data_list)
    
    # 저장 후 데이터 리스트 초기화
    data_list = []

# 입력 값을 리스트에 저장하거나 수정하는 함수
def store_value():
    global data_list, max_entries, current_index, selected_value, current_entry_count

    try:
        perception = selected_value.get()
        if perception == 0:
            raise ValueError

        max_force = force_map[perception]  # MaxForce 값을 선택한 인지 강도에 따라 설정
        timestamp = time.strftime('%Y-%m-%d %H:%M:%S')

        if current_index is not None:
            data_list[current_index] = [name, perception, max_force, timestamp]
            current_index = None
        else:
            data_list.append([name, perception, max_force, timestamp])
        
        # 입력 횟수 증가 및 라벨 갱신
        current_entry_count += 1
        update_entry_label()

        if len(data_list) >= max_entries:
            save_data()
            messagebox.showinfo("저장 완료", f"{max_entries}개의 데이터가 저장되었습니다.")
        
        # 체크박스 초기화
        selected_value.set(0)
    except ValueError:
        messagebox.showerror("입력 오류", "값을 선택해주세요.")

# 이전 값을 불러와 수정하는 함수
def edit_previous():
    global current_index, selected_value, current_entry_count
    if data_list:
        current_index = len(data_list) - 1
        selected_value.set(data_list[current_index][1])
        
        # 입력 횟수 감소 및 라벨 갱신
        if current_entry_count > 0:
            current_entry_count -= 1
        update_entry_label()
    else:
        messagebox.showerror("오류", "수정할 데이터가 없습니다.")

# 현재 입력 횟수를 업데이트하는 함수
def update_entry_label():
    global current_entry_count, max_entries
    entry_label.config(text=f"Entry: {current_entry_count + 1} / {max_entries}")

# 사용자가 선택한 값을 처리하는 함수
def on_value_selected():
    store_value()

# GUI 창을 종료할 때 호출되는 함수
def on_closing():
    if data_list:
        save_data()
    root.destroy()

# 사용자의 이름을 받는 창
def get_name():
    global name
    
    def submit_name():
        global name
        name = name_entry.get().strip()
        if name:
            name_window.destroy()
            show_main_window()
        else:
            messagebox.showerror("입력 오류", "이름을 입력해주세요.")
    
    name_window = tk.Tk()
    name_window.title("정보 입력")
    name_window.geometry("300x150")
    
    tk.Label(name_window, text="이름_렌더링 방식을 입력하세요:", font=("Arial", 12)).pack(pady=10)
    name_entry = tk.Entry(name_window, font=("Arial", 12))
    name_entry.pack(pady=5)
    
    tk.Button(name_window, text="확인", command=submit_name, font=("Arial", 12)).pack(pady=10)
    
    name_window.mainloop()

# 메인 입력 창을 표시하는 함수
def show_main_window():
    global root, selected_value, entry_label
    root = tk.Tk()
    root.title("Perception Data Entry")
    root.geometry("500x250")

    # 입력 횟수 라벨
    entry_label = tk.Label(root, text=f"Entry: 1 / {max_entries}", font=("Arial", 12))
    entry_label.pack(anchor='nw', padx=10, pady=10)

    frame = tk.Frame(root)
    frame.place(relx=0.5, rely=0.5, anchor=tk.CENTER)

    # 입력 라벨
    instruction_label = tk.Label(frame, text="느껴지는 인지 강도를 선택해주세요:", font=("Arial", 14))
    instruction_label.pack(pady=10)

    # 체크박스 선택값을 저장할 변수
    selected_value = tk.IntVar()
    selected_value.set(0)  # 초기값 0 설정 (아무 것도 선택되지 않은 상태)

    # 체크박스와 라벨을 수평으로 배치할 프레임
    checkbox_frame = tk.Frame(frame)
    checkbox_frame.pack(pady=10)

    # 왼쪽에 1(soft) 라벨
    soft_label = tk.Label(checkbox_frame, text="Soft", font=("Arial", 15))
    soft_label.pack(side=tk.LEFT, padx=10)

    # 체크박스 버튼 생성
    for i in range(1, 6):
        rb = tk.Radiobutton(
            checkbox_frame, 
            text=str(i), 
            variable=selected_value, 
            value=i, 
            font=("Arial", 15), 
            indicatoron=0,  # 버튼 스타일을 사용하여 크기 조절
            width=3,        # 체크박스의 너비
            height=2,       # 체크박스의 높이
            command=on_value_selected  # 선택 시 자동으로 다음 trial로 넘어가도록 설정
        )
        rb.pack(side=tk.LEFT, padx=5)

    # 오른쪽에 5(hard) 라벨
    hard_label = tk.Label(checkbox_frame, text="Hard", font=("Arial", 15))
    hard_label.pack(side=tk.LEFT, padx=10)

    # 버튼을 가로로 나열할 프레임
    button_frame = tk.Frame(frame)
    button_frame.pack(pady=10)

    # 이전 버튼
    button_edit = tk.Button(button_frame, text="이전", command=edit_previous, font=("Arial", 12))
    button_edit.pack(side=tk.LEFT, padx=5)

    # 창 닫을 때의 동작 설정
    root.protocol("WM_DELETE_WINDOW", on_closing)

    root.mainloop()

# 프로그램 시작 시 이름을 입력받는 창을 띄움
get_name()
