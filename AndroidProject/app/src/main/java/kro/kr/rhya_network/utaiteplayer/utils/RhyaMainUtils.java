package kro.kr.rhya_network.utaiteplayer.utils;

import java.util.ArrayList;
import java.util.Random;

import kro.kr.rhya_network.utaiteplayer.BuildConfig;

public class RhyaMainUtils {
    // 앱 버전 확인
    public boolean updateChecker(String version) {
        return Integer.parseInt(BuildConfig.VERSION_NAME.replace(".", "")) >= Integer.parseInt(version.replace(".", ""));
    }


    // 앱 버전
    public String getAppVersion() {
        return BuildConfig.VERSION_NAME;
    }


    // 랜덤 시간 대기
    public void randomThreadSleep(int input) throws InterruptedException {
        Random random = new Random();
        Thread.sleep(random.nextInt(input));
    }


    // 구독 정렬
    public ArrayList<RhyaMusicInfoVO> sortRhyaMusicInfoVOArrayList(ArrayList<RhyaSingerDataVO> rhyaSingerDataVOArrayList, ArrayList<RhyaMusicInfoVO> rhyaMusicInfoVOArrayList) {
        ArrayList<RhyaMusicInfoVO> newRhyaMusicInfoVOArrayList = rhyaMusicInfoVOArrayList;

        int index = 0;
        int nowIndex = 0;

        for (RhyaMusicInfoVO rhyaMusicInfoVO : rhyaMusicInfoVOArrayList) {
            boolean isChange = false;

            for (RhyaSingerDataVO rhyaSingerDataVO : rhyaSingerDataVOArrayList) {

                assert rhyaMusicInfoVO != null;
                if (rhyaSingerDataVO.getUuid().equals(rhyaMusicInfoVO.getSingerUuid())) {

                    isChange = true;

                    RhyaMusicInfoVO temp = rhyaMusicInfoVOArrayList.get(index);

                    newRhyaMusicInfoVOArrayList.set(index, rhyaMusicInfoVO);
                    newRhyaMusicInfoVOArrayList.set(nowIndex, temp);

                    index++;

                    break;
                }

            }

            if (!isChange) {
                newRhyaMusicInfoVOArrayList.set(nowIndex, rhyaMusicInfoVO);
            }

            nowIndex ++;
        }

        return newRhyaMusicInfoVOArrayList;
    }
}
