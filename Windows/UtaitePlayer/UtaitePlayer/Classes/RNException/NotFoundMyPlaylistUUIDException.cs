using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace UtaitePlayer.Classes.RNException
{
    public class NotFoundMyPlaylistUUIDException : Exception
    {
        // 기본 생성자
        public NotFoundMyPlaylistUUIDException() : base() { }
        // 에러 메시지를 포함하는 생성자
        public NotFoundMyPlaylistUUIDException(string message) : base(message) { }
        // 에러 메시지와 내부 예외를 포함하는 생성자
        public NotFoundMyPlaylistUUIDException(string message, Exception e) : base(message, e) { }
        // 입력 스트림을 이용하는 생성자
        public NotFoundMyPlaylistUUIDException(SerializationInfo info, StreamingContext cxt) : base(info, cxt) { }
        // 오류 데이터
        public string UUID { get; set; }
    }
}
