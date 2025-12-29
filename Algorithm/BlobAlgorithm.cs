using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JH_VisionProject.Algorithm
{
        public struct BinaryThreshold
        {
            public int lower { get; set; }
            public int upper { get; set; }
            public bool invert { get; set; }
        }

        public class BlobAlgorithm : InspAlgorithm
        {
            public BinaryThreshold BinThreshold { get; set; } = new BinaryThreshold();

            public BlobAlgorithm()
            {
                InspectType = InspectType.InspBinary;
            }

            //InspAlgorithm을 상속받아, 구현하고, 인자로 입력받던 것을 부모의 _srcImage 이미지 사용
            //검사 시작전 IsInspected = false로 초기화하고, 검사가 정상적으로 완료되면,IsInspected = true로 설정
            public override bool DoInspect()
            {
                ResetResult();

                IsInspected = true;

                return true;
            }

            //검사 결과 초기화
            public override void ResetResult()
            {
                base.ResetResult();
            }
        }

}
