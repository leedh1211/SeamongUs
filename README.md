# Seamong us


<br><br><br>  <img src="https://github.com/user-attachments/assets/48c41066-5a94-47ec-bbc9-748bda38d55b" width="500"> <br><br>




> *"모두를 제거하고, 자원을 독차지하겠어."*

---
## 📌 목차  
1. [프로젝트 소개](#프로젝트-소개)  
2. [팀 소개](#팀-소개)  
3. [프로젝트 계기](#프로젝트-계기)  
4. [주요기능](#주요기능)  
5. [개발기간](#개발기간)  
6. [기술적 의사결정](#기술적-의사결정)  
7. [서비스 구조](#서비스-구조)  
8. [와이어프레임](#와이어프레임)  
9. [database](#database)
10. [트러블 슈팅](#트러블-슈팅)

<br>

---
## 프로젝트 소개

<br>

> `Seamong Us`는 표류된 생존자들의 심리전과 협동을 핵심으로, **멀티 플레이를 통한 플레이어간 추리가 필수적인 요소입니다.**
>
> - Seamong Us는 바다에 표류한 생존자들이 펼치는 심리전과 협동을 중심으로 개발
> - 임포스터와 크루메이트 간 긴장감 넘치는 역할 대립  
> - 실시간 멀티플레이어 상호작용과 전략적 팀워크 강조

<br>

### 프로젝트 설명
| 항목                | 내용 |
|---------------------|------|
| 🏷️ **프로젝트 명**    | `Seamong Us` <br> (Sea + Among Us의 결합어) |
| 📝 **팀 이름**     | 쉬고10조 |
| 🎮 **장르**          | 멀티플레이어 심리 추리 게임 (Social Deduction) |
| 💡 **조작 방법**          | 이동 : WASD <br> 점프 : Space <br> 상호 작용 : E 혹은 마우스 클릭 <br> 감정 표현 : G |
| 🏝️ **제작 배경**     | 기존 어몽어스 장르의 시스템을 **생존 테마**로 재해석하여 <br> 더 강한 **몰입감과 전략적 재미**를 제공 |
| 💻 **개발 목표 플랫폼** | PC 🖥️<br>Unity + Photon 🛠️ 기반 멀티플레이 |

<br>

### 주요 컨셉 요약

| 항목           | 내용                                                                                     |
|----------------|------------------------------------------------------------------------------------------|
| 🎨 **배경**      | 오래 전, 바다 위의 어느 섬 작은 동굴엔 값비싼 자원이 있다는 소문이 떠돌았다.<br> 사람들은 그것을 찾아내기 위해 이 섬에 모였지만 자원은 끝내 발견되지 않았고, 그들은 탈출하지도 못한 채 섬에 갇혀 함께 살아갈 뿐이었다.<br> 그러던 어느 날, 갑작스레 동굴이 무너져내렸다.<br> 분명 누군가가 일부러 그런 것이다. 자원을 찾은 자가, 증거를 감추기 위해...<br> 주민들은 서로를 의심하기 시작했다. 분명 음모일 거라고, 계획된 일이라고.<br> 그들의 갈등은 나날이 커져갔고, 서로를 더욱 믿지 못하게 되어갔다.<br> 그리고 마침내, 누군가 결심하였다.<br> "모두를 제거하고, 자원을 독차지하겠어."<br> 그것이, 이 섬에 피어난 절망의 시작점이었다...|
| 🎭 **역할 분배**  | 🔪 **Imposter** - 정체를 숨기고 미션 방해 및 크루원 암살<br>🛠️ **Crewmate** - 임포스터 색출 및 미션 완수 |
| 🎯 **미션**      | 🎮 다양한 미니게임으로 구성된 크루원의 승리 조건                                         |
| 🍄 **아이템**    | 🍄 **버섯** - 체력 일정량 회복<br>🥩 **고기** - 일정 시간 이동 속도 증가                    |
| 🗳️ **투표**     | 🤝 회의와 투표를 통해 임포스터를 색출하는 시스템       

<br>

---

<br>

## 팀 소개
| 이름 | 역할 | 담당 업무| GitHub | Blog |
|------|------|--------|-------|-------|
| 김웅진 | 팀장 | 아이템 / 인벤토리 / 인트로 씬 / 채팅 / 네트워크 관련 / 플레이어 체력 | [https://github.com/KUJ1031](https://github.com/KUJ1031) | https://thsusekdnlt1.tistory.com/ |
| 이동헌 | 팀원 | DB,서버,네트워크 전반 / 멀티 동기화 / GIT관리 및 씬 병합 / 게임 흐름 총괄 | [https://github.com/leedh1211](https://github.com/leedh1211) | https://leedh12.tistory.com/ |
| 강민성 | 팀원 | 플레이어 / 크루원, 임포스터 / 고스트 / 상호작용 / 채팅 | [https://github.com/mcas0215](https://github.com/mcas0215) | https://mcas0215.tistory.com/ |
| 최진안 | 팀원 | 미션 퍼즐 전반 / 충돌 감지 처리 / 상호작용 / UI 보조 | [https://github.com/hhd14725](https://github.com/hhd14725) | https://muchmercy.tistory.com/manage/posts |
| 이선량 | 팀원 | 타일 맵 구성 / 시네머신 / 사운드 / 미션 퍼즐 보조 / 투표 로직 보조 | [https://github.com/AgathaYi](https://github.com/AgathaYi) | https://05cm.tistory.com/ |
| 손양복 | 팀원 | UI/UX 전반 / 투표 이후 추방 UI / 팝업 요소 및 디자인 | [https://github.com/YBdhhh](https://github.com/YBdhhh) | https://97926.tistory.com/ |

<br>

---

<br>

## 프로젝트 계기
> *초기 팀 프로젝트 구상 계획*
<img src="https://github.com/user-attachments/assets/394c89dd-a4a2-41bb-8d80-7c9f52f250ac" width="405">
<img src="https://github.com/user-attachments/assets/fd2f515f-106d-4db1-9a32-913409c1599a" width="500">

<br><br>

> *장르 선택 계기*

| 📌 경험 배경                                  | 💡 도전 계기                                                  |
|-----------------------------------------------|----------------------------------------------------------------|
| 🎮 Unity 기반 싱글플레이 프로젝트 중심의 학습         | 🕹️ 실시간 상호작용과 다인 동기화에 대한 실무 경험 부족               |
| 📘 Photon 등 네트워크 기능 학습 기회 부족            | 🌐 멀티플레이 구현 능력 향상을 위한 실질적 프로젝트 기획               |
| 🛠️ 기본적인 게임 시스템 구현 경험 축적              | 🚀 더 높은 기술 난이도와 협업 요구가 있는 분야로의 도전                |

<br>

- Unity를 기반으로 다양한 분야의 게임을 개발하며 기본적인 게임 시스템 구현 역량을 쌓아왔지만, **네트워크 및 멀티플레이** 제작 경험이 부족했습니다.
- 이번 프로젝트를 통해 이를 익히며 보완하고 **보다 높은 기술 난이도와 협업 중심 개발 역량**을 갖추고자 본 프로젝트를 기획하게 되었습니다.

---

<br>

## 주요기능

<details>
<summary> 방 입장 이전</summary>
<br>

- 🟢 회원가입 / 로그인
<img src="https://github.com/user-attachments/assets/b482792d-206f-4438-b7d1-a9308bef7749" width="500">

<br><br>

- 🟢 방 생성
<img src="https://github.com/user-attachments/assets/751b89a2-4087-4fcb-be51-0720027c2156" width="500">

<br><br>

- 🟢 방 입장
<img src="https://github.com/user-attachments/assets/37c6f073-3407-4d13-b996-0547f32f9ed5" width="500">

<br><br>

</details>

<details>
<summary> 방 입장 ~ 게임 시작 이전</summary>
<br>
    
- 💬 실시간 채팅
<img src="https://github.com/user-attachments/assets/2ba53a76-1345-451d-9be9-c92bcb30461f" width="500">

<br><br>

- 🎭 코스튬
<img src="https://github.com/user-attachments/assets/dbf93bf3-a1c5-4a41-aa5c-8ce85c8a4e26" width="500">

<br><br>

</details>

<details>

<summary> 게임 시작 ~ 게임 종료 이전</summary>
<br>

- 🎁 아이템 획득 및 사용 (All)
  
<img src="https://github.com/user-attachments/assets/90c29c40-b171-46eb-ba2e-c642a73370d6" width="500"><br>
<img src="https://github.com/user-attachments/assets/9e5008d2-202f-4139-bdea-a3570c79d236" width="500"><br>

<br><br>

- 🗡 플레이어 암살 (Imposter)
  
<br>

1️⃣ 임포스터 시점

<img src="https://github.com/user-attachments/assets/8032f8f5-af40-45c5-8712-5cad42839fca" width="500">

<br>

2️⃣ 피해자 시점

<img src="https://github.com/user-attachments/assets/335daf82-3158-4619-96ce-c82e00bf2364" width="500">

<br>
<br>

- 🧩 미션 탐색 및 수행 (Crewmate)<br><br>

1️⃣ 빨래 널기

<img src="https://github.com/user-attachments/assets/f7068eb7-6adc-4034-b96d-3cf16d8f01e6" width="500">

<br><br>

2️⃣ 돌멩이 치우기

<img src="https://github.com/user-attachments/assets/adf7fecf-ef5a-480b-9310-089e02723524" width="500">

<br><br>

3️⃣ 양 데려오기

<img src="https://github.com/user-attachments/assets/8bc274ce-aab5-4491-a8ca-423a79c46c59" width="500">

<br><br>

4️⃣ 이정표 수리하기

<img src="https://github.com/user-attachments/assets/e4f67122-974b-42c3-a2a2-6d2aa7921b19" width="500">

<br><br>

- 🚨 시체 발견 및 신고 (Crewmate)
<img src="https://github.com/user-attachments/assets/e7bccd1d-f6b3-4891-8b7d-e5a806ae6c9c" width="500">

<br><br>

- 🗳 회의 및 투표 진행 (All)
<img src="https://github.com/user-attachments/assets/0fa01a3a-f976-436c-ab0f-9b3fe0b01419" width="500">

<br><br>

</details>

<details>
<summary> 게임 종료 이후</summary>
<br>


- 🏆 승리 조건 체크
<img src="https://github.com/user-attachments/assets/fdc35290-f856-47cc-a44f-6c4f0cbb51d5" width="500">

<br><br>

- 🔁 로비 재입장

<img src="https://github.com/user-attachments/assets/64790bbb-464f-4ff5-ab76-75afa453e79b" width="500">

<br><br>

---

<br>

</details>

<br>

## 개발기간

- **총 개발 기간**: 2025.06.11 ~ 2025.06.18
  
<br>

---

<br>

## 기술적 의사결정

> *기술 요구 사항*
### 🌐 서버 통신 - Photon PUN2
- 실시간 멀티플레이어 네트워킹, 룸 생성/입장, 플레이어 동기화, RPC 호출 등 지원  
- Unity와의 높은 호환성, 무료 요금제에서도 충분한 기능 제공  
- 플레이어 간 위치/애니메이션 동기화, 채팅, 아이템 획득 이벤트 공유 구현

### 🔄 게임 흐름 분기 - EventBus / EventCode
- 각 모듈 간 결합도 최소화 및 의존성 제거  
- 다양한 게임 상황을 코드 흐름상에서 분기 처리 가능  
- 이벤트 기반 아키텍처로 유지보수 및 확장성 향상

### 📡 데이터 전달 - JSON
- 직렬화/역직렬화를 통해 서버와의 데이터 통신 구현  
- 간단하고 가벼운 포맷으로 구조화된 정보 전달  
- 클라이언트-서버 간 게임 상태, 캐릭터 정보 전달 등에 활용

### 🧱 아이템 시스템 - ScriptableObject
- 아이템 정보(이름, 설명, 스탯 등)를 Data 자산으로 분리  
- 런타임에 ScriptableObject 데이터를 기반으로 객체 생성  
- 코드와 데이터의 분리로 관리 용이, 확장성 높은 아이템 구조 구성

### 🔁 실시간 로직 흐름 - Observer Pattern
- UI, 상태, 리소스 등 변화 감지 및 반응 처리  
- 이벤트 기반으로 각 모듈 간의 직접 참조 없이 동기화 가능  
- 유지보수성 향상 및 기능 확장 시 유연한 대응 가능

<br>

> *사용 기술 스택*
### 🎥 카메라 설정 - Cinemachine
- 자연스럽고 부드러운 카메라 이동 구현  
- 플레이어를 따라다니는 추적 카메라, 줌 인/아웃, 전환 효과 등 유연하게 설정 가능  
- 씬 연출 및 몰입감을 높이는 데 기여

### 🌍 전역 접근성 - Singleton
- 인스턴스 단일화를 통한 전역 관리 구조 설계  
- 전역에서 접근 가능한 Manager 관련 로직 구현  
- 리소스 절약 및 상태 공유 간소화

### 🧩 프로젝트 확장 - OCP (Open-Closed Principle)
- 클래스는 확장에는 열려 있고, 변경에는 닫혀 있어야 한다는 원칙 적용  
- 기능 추가 시 기존 코드를 수정하지 않고 새로운 기능을 덧붙이는 구조  
- 유지보수성과 확장성을 모두 고려한 설계 방식

### 🏭 객체 생성 로직 캡슐화 - Factory Pattern
- 객체 생성 책임을 분리하여 클라이언트 코드로부터 생성 로직 숨김  
- 다양한 타입의 객체를 유연하게 생성 가능  
- 결합도 최소화 및 코드 재사용성 향상

<br>

---

<br>

## 서비스 구조

> *FlowChart (로직 흐름도)*
<img src="https://github.com/user-attachments/assets/6701470f-67af-4d80-b216-124723f7006b" width="500">

<br>


## 와이어프레임

> *초기 UI 구상*
<img src="https://github.com/user-attachments/assets/1bf549d5-43c7-4376-acd3-cd26abc405af" width="700">

<br><br>

> *초기 맵 설계도*
<img src="https://github.com/user-attachments/assets/c985eb99-d182-4608-9c85-1697e90ed805" width="500">

---

## DataBase
> *회원가입 및 로그인 - DataBase 연동 과정*
<img src="https://github.com/user-attachments/assets/c640e41d-cc84-4806-82f4-b81d92b30540" width="800">

---

<br>

## 트러블 슈팅

<br>

<details>
<summary> 김웅진 트러블 슈팅 사항</summary>

<img src="https://github.com/user-attachments/assets/760e13a6-bbac-4b31-9441-22151758cd5e" width="500">

</details>

<details>
<summary> 이동헌 트러블 슈팅 사항</summary>

<img src="https://github.com/user-attachments/assets/df0317b4-305e-46d7-83e3-89fcbe8bd53a" width="500">

</details>

<details>
<summary> 강민성 트러블 슈팅 사항</summary>

<img src="https://github.com/user-attachments/assets/e1442451-5673-4831-9577-61a7d707d511" width="500">



</details>

<details>
<summary> 최진안 트러블 슈팅 사항</summary>

<img src="https://github.com/user-attachments/assets/5a67a358-af70-4644-bebd-ca27b93a3792" width="500">

</details>

<details>
<summary> 이선량 트러블 슈팅 사항</summary>

<img src="https://github.com/user-attachments/assets/e884f95a-dbc5-44c2-85b4-6acae83185e8" width="500">


</details>

<details>
<summary> 손양복 트러블 슈팅 사항</summary>

<img src="https://github.com/user-attachments/assets/211f495e-6ece-499e-b7d3-0b2f630c3ed1" width="500">

</details>

<br> 

---
