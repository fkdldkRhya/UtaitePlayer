package kro.kr.rhya_network.utaiteplayer.utils;

public class RhyaMusicInfoVO {
    private String uuid;
    private String name;
    private String time;
    private String lyrics;
    private String singer;
    private String singerUuid;
    private String singerImage;
    private String songWriter;
    private String image;
    private String mp3;
    private String type;
    private String date;
    private int version;


    public RhyaMusicInfoVO(String uuid, String name, String time, String lyrics, String singer, String singerUuid, String singerImage, String songWriter, String image, String mp3, String type, String date, int version) {
        this.uuid = uuid;
        this.name = name;
        this.time = time;
        this.lyrics = lyrics;
        this.singer = singer;
        this.singerUuid = singerUuid;
        this.singerImage = singerImage;
        this.songWriter = songWriter;
        this.image = image;
        this.mp3 = mp3;
        this.type = type;
        this.date = date;
        this.version = version;
    }


    public String getUuid() {
        return uuid;
    }

    public String getName() {
        return name;
    }

    public String getTime() {
        return time;
    }

    public String getLyrics() {
        return lyrics;
    }

    public String getSinger() {
        return singer;
    }

    public String getSingerUuid() {
        return singerUuid;
    }

    public String getSingerImage() {
        return singerImage;
    }

    public String getSongWriter() {
        return songWriter;
    }

    public String getImage() {
        return image;
    }

    public String getMp3() {
        return mp3;
    }

    public String getType() {
        return type;
    }

    public String getDate() {
        return date;
    }

    public int getVersion() {
        return version;
    }


    public void setUuid(String uuid) {
        this.uuid = uuid;
    }

    public void setName(String name) {
        this.name = name;
    }

    public void setTime(String time) {
        this.time = time;
    }

    public void setLyrics(String lyrics) {
        this.lyrics = lyrics;
    }

    public void setSinger(String singer) {
        this.singer = singer;
    }

    public void setSingerUuid(String singerUuid) {
        this.singerUuid = singerUuid;
    }

    public void setSingerImage(String singerImage) {
        this.singerImage = singerImage;
    }

    public void setSongWriter(String songWriter) {
        this.songWriter = songWriter;
    }

    public void setImage(String image) {
        this.image = image;
    }

    public void setMp3(String mp3) {
        this.mp3 = mp3;
    }

    public void setType(String type) {
        this.type = type;
    }

    public void setDate(String date) {
        this.date = date;
    }

    public void setVersion(int version) {
        this.version = version;
    }
}
