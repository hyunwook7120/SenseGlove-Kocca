import pandas as pd
import os
import ast

# # 파일이 저장된 디렉토리 경로 (예시)
# directory_path = './data'

# # 디렉토리에서 파일 리스트 가져오기
# files = os.listdir(directory_path)

# # 파일 이름에서 중복되지 않은 기본 이름 추출 (예: 'taeyoon_T')
# base_names = set(f.split('.')[0] for f in files)

# # 각 기본 이름에 대해 txt와 csv 파일을 찾아서 처리
# for base_name in base_names:
#     txt_file_path = os.path.join(directory_path, f"{base_name}.txt")
#     csv_file_path = os.path.join(directory_path, f"{base_name}.csv")
    
#     # 두 파일이 모두 존재하는지 확인
#     if os.path.exists(txt_file_path) and os.path.exists(csv_file_path):
#         # CSV 파일 읽기
#         csv_df = pd.read_csv(csv_file_path)

#         # TXT 파일 읽기 및 데이터 처리
#         with open(txt_file_path, 'r') as file:
#             lines = file.readlines()

#         # result와 rendering 열로 나누기
#         result_rendering_data = [ast.literal_eval(line.strip()) for line in lines]
#         txt_df = pd.DataFrame(result_rendering_data, columns=['result', 'rendering'])

#         # 두 데이터프레임 합치기
#         combined_df = pd.concat([csv_df, txt_df], axis=1)

#         # 결과 파일로 저장
#         output_file_path = os.path.join(directory_path, f"{base_name}_combined.csv")
#         combined_df.to_csv(output_file_path, index=False)

#         print(f"Successfully combined and saved: {output_file_path}")
#     else:
#         print(f"Missing either txt or csv file for base name: {base_name}")


# 파일이 저장된 디렉토리 경로 (예시)
directory_path = './'

# 디렉토리에서 모든 CSV 파일 리스트 가져오기
csv_files = [f for f in os.listdir(directory_path) if f.endswith('.csv')]

# 각 CSV 파일에 대해 작업 수행
for csv_file in csv_files:
    file_path = os.path.join(directory_path, csv_file)
    
    # CSV 파일 읽기
    df = pd.read_csv(file_path)
    
    # 모든 열의 순서를 지정하되, timestamp를 마지막으로 이동
    cols = df.columns.tolist()  # 전체 열 리스트를 가져옵니다.
    cols.append(cols.pop(cols.index('timestamp')))  # timestamp 열을 마지막으로 이동
    
    # 새로운 열 순서로 데이터프레임 재배열
    df = df[cols]
    
    # 결과를 같은 이름으로 다시 저장 (덮어쓰기)
    df.to_csv(file_path, index=False)
    
    print(f"Processed and saved: {csv_file}")
