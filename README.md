# FaceLib (com.face.facelib)

Unity에서 재사용 가능한 유틸리티를 제공하는 UPM 패키지입니다. Git(UPM) 또는 Git 서브모듈로 설치할 수 있습니다.

## 설치

### 옵션 1: Git Submodule (버전 고정에 권장)

git submodule add https://github.com/faces0312/FaceLib.git Packages/com.face.facelib

git submodule update --init --recursive

### 옵션 2: UPM Git URL
`Packages/manifest.json`의 dependencies에 추가:

{
  "dependencies": {
    "com.face.facelib": "https://github.com/faces0312/FaceLib.git#v1.1.0"
  }
}

## 사용법
아래는 기본적인 사용 예시입니다.

기본 API:

using FaceLib;

// 표준화된 포맷으로 로그 출력
FaceLogger.Log("Hello FaceLib!");
 

### GameManager 사용 예시

```csharp
using FaceLib;
using UnityEngine;

public class GameManager : BaseGameManager
{
	protected override void AddManagers()
	{
		// 리소스/오브젝트/풀 매니저 구성
		_managerList.Add(ResourceManager.Instance);   // 사용자가 구현한 리소스 매니저(선택)
		_managerList.Add(PoolManager.Instance);       // FaceLib 풀 매니저
		_managerList.Add(ObjectManager.Instance);     // FaceLib 오브젝트 매니저
	}

	protected override void OnInit()
	{
		// 초기 세팅 (등록/프리웜 등)
		// ObjectManager.Instance.RegisterPrefab("Book", bookPrefab);
		// ObjectManager.Instance.Prewarm("Book", 16, poolRoot);
	}

	// public GameObject bookPrefab;
	// public Transform poolRoot;
}
```

## 요구사항
- Unity 2020.3+

## 개발
- 런타임 코드는 `Runtime/`에 위치합니다.
- 에디터 전용 코드는 `Editor/`에 위치합니다.