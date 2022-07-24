package kro.kr.rhya_network.utaiteplayer.utils;

import java.util.ArrayList;

public class RhyaPlayList {
    private String uuid;
    private String name;
    private ArrayList<String> musicList;
    private int imageType;


    public ArrayList<String> getMusicList() {
        return musicList;
    }

    public void setMusicList(ArrayList<String> musicList) {
        this.musicList = musicList;
    }

    public int getImageType() {
        return imageType;
    }

    public void setImageType(int imageType) {
        this.imageType = imageType;
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public String getUuid() {
        return uuid;
    }

    public void setUuid(String uuid) {
        this.uuid = uuid;
    }
}
