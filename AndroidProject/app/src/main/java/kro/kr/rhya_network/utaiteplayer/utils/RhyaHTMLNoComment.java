package kro.kr.rhya_network.utaiteplayer.utils;

public class RhyaHTMLNoComment {
    public String getNoComment(String data) {
        return data.replaceAll("<!--.*-->","").replace("\r\n", "").replace(System.lineSeparator(), "");
    }
}
