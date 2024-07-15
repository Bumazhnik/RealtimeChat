# RealtimeChat
실시간 채팅 애플리케이션

.NET 8, npm 필요합니다.
 


## 빌드
프로젝트 폴도로 이동
```bat
cd RealtimeChat
```

"Database" 폴도 생성
```bat
mkdir Database
```
"sensitiveConfig.json" 파일을 만들고 이 내용 작성
```json
{
  "ConnectionString": "Data Source=.\\Database\\applicationcontextdb.db"
}
```

npm 패키지 설치
```bat
npm install
```
TypeScript랑 CSS 빌드(묶기)
```bat
npm run build
```

프로젝트 실행
```bat
dotnet run
```
