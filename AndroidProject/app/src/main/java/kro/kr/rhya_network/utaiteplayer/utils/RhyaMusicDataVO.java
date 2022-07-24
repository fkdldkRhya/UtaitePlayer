package kro.kr.rhya_network.utaiteplayer.utils;

import java.util.ArrayList;

public class RhyaMusicDataVO {
    private ArrayList<RhyaSingerDataVO> subscribeList;


    public RhyaMusicDataVO(ArrayList<RhyaSingerDataVO> subscribeList) {
        this.subscribeList = subscribeList;
    }


    public ArrayList<RhyaSingerDataVO> getSubscribeList() {
        return subscribeList;
    }
    public void setSubscribeList(ArrayList<RhyaSingerDataVO> subscribeList) {
        this.subscribeList = subscribeList;
    }
}
