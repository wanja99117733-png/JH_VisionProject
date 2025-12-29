using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;


namespace JH_VisionProject.Algorithm
{
    public enum InspectType
    {
        InspNone = -1,
        InspBinary,
        InspCount
    }
    //추상화 클래스
    public abstract class InspAlgorithm
    {
        // 알고리즘 타입 정의
        public InspectType InspectType { get; set; } = InspectType.InspNone;

        // 알고리즘 사용할지 여부 결정

        public bool IsUse { get; set; } = true;
        // 검사가 완료되었는지 여부
        public bool IsInspected { get; set; } = false;

        //검사할 원본 이미지
        protected Mat _srcImage = null;

        //검사 결과 정보
        public List<string> ResultString { get; set; } = new List<string>();

        //불량 여부
        public bool IsDefect { get; set; }

        //검사 함수로, 상속 받는 클래스는 필수로 구현해야한다.
        public abstract bool DoInspect();

        //검사 결과 정보 초기화
        public virtual void ResetResult()
        {
            IsInspected = false;
            IsDefect = false;
            ResultString.Clear();
        }
    }
}
