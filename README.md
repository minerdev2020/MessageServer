# MessageServer
C#으로 소켓을 이용해서 만든 간단한 비동기 메신저 서버입니다.

## Feature
1. 회원가입
2. 로그인
3. 로그아웃
4. 내 이름 수정
5. 친구 리스트 조회
6. 친구 추가
7. 채팅방 리스트 조회
8. 채팅방 생성
9. 채팅방 참가
10. 채팅방 떠나기
11. 채팅방 타이틀 수정
12. 채팅 메시지 전송

## DB Table
1. user (가입한 유저의 정보를 담고 있는 테이블입니다)
    * id (primary key)
    * email (unique key)
    * password
    * online
    * ip
    
2. room (생성한 채팅방의 정보를 담고 있는 테이블입니다)
    * id (primary key)
    * title
    
3. user_friend (유저간의 친구 관계를 담고 있는 테이블입니다)
    * user_id (foreign key)
    * friend_id (foreign key)
    
4. user_room (유저와 채팅방간의 관계를 담고 있는 테이블입니다)
    * user_id (foreign key)
    * room_id (foreign key)
