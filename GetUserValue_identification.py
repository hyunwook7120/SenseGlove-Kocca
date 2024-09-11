import tkinter as tk
from tkinter import messagebox
import csv
import os
import time

name = ""
data_list = []
max_entries = 100
current_index = None
current_entry_count = 0
selected_force = None
answer_list = []  # 정답 리스트를 저장할 변수
show_answer_flag = False  # 정답을 보여줄지 여부를 결정하는 플래그

# MaxForce 값을 매핑하는 딕셔너리
force_map = {
    1: 0.0,
    2: 0.1,
    3: 0.3,
    4: 0.5,
    5: 1.0
}

# 정답 파일을 읽고 리스트로 변환하는 함수
def load_answers():
    global answer_list
    script_dir = os.path.dirname(os.path.abspath(__file__))  # 스크립트의 절대 경로
    filepath = os.path.join(script_dir, 'Assets', 'Resources', 'Log.txt')
    with open(filepath, 'r') as file:
        lines = file.readlines()
        for line in lines:
            answer = tuple(map(float, line.strip().strip('()').split(', ')))
            answer_list.append(answer)

# 정답을 보여주는 함수
def show_correct_answer():
    global current_entry_count, selected_force
    if current_entry_count < len(answer_list):
        correct_answer = answer_list[current_entry_count][0]
        expected_choice = None
        
        # force_map을 역매핑해서 정답을 추정
        for key, value in force_map.items():
            if value == correct_answer:
                expected_choice = key
                break
        
        if expected_choice is not None:
            # 폰트를 Bold로 하고 크기를 16으로 설정
            answer_label.config(text=f"정답은: {expected_choice}번 입니다.", font=("Arial", 16, "bold"))
        else:
            answer_label.config(text="정답을 찾을 수 없습니다.", font=("Arial", 16, "bold"))
    else:
        answer_label.config(text="정답이 없습니다.", font=("Arial", 16, "bold"))


# 정답 라벨을 초기화하는 함수
def clear_answer():
    answer_label.config(text="")

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

# 데이터 저장 또는 수정을 처리하는 함수
def store_value():
    global data_list, max_entries, current_index, current_entry_count, selected_force

    try:
        if selected_force is None:
            raise ValueError("값을 선택해주세요.")

        max_force = force_map[selected_force]  # MaxForce 값을 선택한 인지 강도에 따라 설정
        timestamp = time.strftime('%Y-%m-%d %H:%M:%S')

        if current_index is not None:
            data_list[current_index] = [name, selected_force, max_force, timestamp]
            current_index = None
        else:
            data_list.append([name, selected_force, max_force, timestamp])

        # 입력 횟수 증가 및 라벨 갱신
        current_entry_count += 1
        update_entry_label()

        if len(data_list) >= max_entries:
            save_data()
            messagebox.showinfo("저장 완료", f"{max_entries}개의 데이터가 저장되었습니다.")
        
        # 선택 초기화 및 정답 라벨 초기화
        selected_value.set(0)
        selected_force = None
        clear_answer()

    except ValueError as e:
        messagebox.showerror("입력 오류", str(e))

# 이전 값을 불러와 수정하는 함수
def edit_previous():
    global current_index, selected_value, current_entry_count, selected_force
    if data_list:
        current_index = len(data_list) - 1
        selected_value.set(data_list[current_index][1])
        selected_force = data_list[current_index][1]
        
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

# 사용자가 선택한 값을 저장하고 정답을 보여주는 함수
def on_value_selected():
    global selected_force
    selected_force = selected_value.get()

    # show_answer_flag가 True일 때만 정답을 보여줌
    if show_answer_flag:
        show_correct_answer()

# GUI 창을 종료할 때 호출되는 함수
def on_closing():
    if data_list:
        save_data()
    root.destroy()

# 사용자의 이름과 실험 모드를 받는 창
# 사용자의 이름과 실험 모드를 받는 창
def get_name():
    global name, show_answer_flag
    
    def submit_name(mode):
        global name, max_entries, show_answer_flag
        name = name_entry.get().strip()
        if name:
            if mode == "co-answer feedback":
                max_entries = 25
                show_answer_flag = True  # 실험 1에서는 정답을 보여줌
            elif mode == "main":
                max_entries = 25
                show_answer_flag = False  # 실험 2에서는 정답을 숨김
            name_window.destroy()
            load_answers()  # 정답 파일 읽기
            show_main_window()
        else:
            messagebox.showerror("입력 오류", "이름을 입력해주세요.")
    
    name_window = tk.Tk()
    name_window.title("정보 입력")
    name_window.geometry("300x200")
    name_window.geometry("300x200")
    
    tk.Label(name_window, text="이름_렌더링 방식을 입력하세요:", font=("Arial", 12)).pack(pady=10)
    name_entry = tk.Entry(name_window, font=("Arial", 12))
    name_entry.pack(pady=5)
    
    # 실험 1과 실험 2 버튼
    button_frame = tk.Frame(name_window)
    button_frame.pack(pady=30)

    tutorial_button = tk.Button(button_frame, text="실험1", command=lambda: submit_name("co-answer feedback"), font=("Arial", 12))
    tutorial_button.pack(side=tk.LEFT, padx=10)

    main_button = tk.Button(button_frame, text="실험2", command=lambda: submit_name("main"), font=("Arial", 12))
    main_button.pack(side=tk.RIGHT, padx=10)
    
    name_window.mainloop()

# 메인 입력 창을 표시하는 함수
def show_main_window():
    global root, selected_value, entry_label, answer_label
    root = tk.Tk()
    root.title("Perception Data Entry")
    root.geometry("500x350")

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
            command=on_value_selected  # 선택 시 정답 표시
        )
        rb.pack(side=tk.LEFT, padx=5)

    # 오른쪽에 5(hard) 라벨
    hard_label = tk.Label(checkbox_frame, text="Hard", font=("Arial", 15))
    hard_label.pack(side=tk.LEFT, padx=10)

    # 버튼을 가로로 나열할 프레임
    button_frame = tk.Frame(frame)
    button_frame.pack(pady=10)

    # 이전 버튼
    button_edit = tk.Button(button_frame, text="이전", command=edit_previous, font=("Arial", 12), state="disabled")
    button_edit.pack(side=tk.LEFT, padx=5)

    # 다음 버튼
    button_next = tk.Button(button_frame, text="다음", command=store_value, font=("Arial", 12))
    button_next.pack(side=tk.RIGHT, padx=5)

    # 정답 라벨
    answer_label = tk.Label(root, text="", font=("Arial", 12))
    answer_label.pack(pady=10)

    # 창 닫을 때의 동작 설정
    root.protocol("WM_DELETE_WINDOW", on_closing)

    root.mainloop()

# 프로그램 시작 시 이름을 입력받는 창을 띄움
get_name()