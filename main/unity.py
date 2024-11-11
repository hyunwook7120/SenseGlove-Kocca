def receving_thermal(txt):
    temperature_list = []
    maxforce_list = []
    rendering_list = []

    with open(txt, 'r') as file:
        for line in file:
            # 각 줄을 쉼표로 분리하고 괄호 제거
            values = line.strip().replace('(', '').replace(')', '').split(',')

            # 첫 번째 값은 maxforce_list에 추가
            maxforce_list.append(float(values[0]))

            # 두 번째 값은 rendering_list에 추가
            rendering_list.append(float(values[1]))

            # 마지막 값(세 번째 값)은 temperature_list에 추가 (정수 또는 float로 변환)
            last_value = float(values[-1])
            temperature_list.append(last_value)

    print("Maxforce List:", maxforce_list)
    print("Rendering List:", rendering_list)
    print("Temperature List:", temperature_list)

    return maxforce_list, rendering_list, temperature_list
