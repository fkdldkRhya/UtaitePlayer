package kro.kr.rhya_network.utaiteplayer.utils;

public class RhyaGetMusicLyrics {
    public String getMusicLyrics(String word, String fullLyrics) {
        // 개행 문자 치환
        fullLyrics = fullLyrics.replaceAll(System.lineSeparator(), " ");

        // 문자열 검색
        int index = fullLyrics.indexOf(word);

        // 검색된 문자열 없음
        if (index == -1) {
            System.out.println("Null");
            return null;
        }

        // 반환 문자열
        StringBuilder lyrics = new StringBuilder();



        // 길이 확인 - 가사 1
        String lyrics1 = null;
        if (index > 100) {
            lyrics1 = fullLyrics.substring(index - 100, index);
        }else {
            lyrics1 = fullLyrics.substring(0, index);
        }

        // 길이 확인 - 가사 2
        String lyrics2 = null;
        if (fullLyrics.length() - index > 100) {
            lyrics2 = fullLyrics.substring(index - word.length(), index + 100);
        }else {
            lyrics2 = fullLyrics.substring(index - word.length());
        }

        // 문자열 합치기
        lyrics.append(lyrics1);
        lyrics.append(lyrics2);

        return lyrics.toString();
    }
}
