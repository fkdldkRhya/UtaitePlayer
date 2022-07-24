package kro.kr.rhya_network.utaiteplayer.lib.com_github_amarjain07_StickyScrollView_1_0_2.provider;

import android.content.Context;
import android.graphics.Point;
import android.util.DisplayMetrics;

import kro.kr.rhya_network.utaiteplayer.lib.com_github_amarjain07_StickyScrollView_1_0_2.provider.interfaces.IScreenInfoProvider;

/**
 * Created by Amar Jain on 17/03/17.
 */
public class ScreenInfoProvider implements IScreenInfoProvider {

    private final Context mContext;

    public ScreenInfoProvider(Context context) {
        mContext = context;
    }

    @Override
    public int getScreenHeight() {
        return getDeviceDimension().y;
    }

    @Override
    public int getScreenWidth() {
        return getDeviceDimension().x;
    }

    Point getDeviceDimension() {
        Point lPoint = new Point();
        DisplayMetrics metrics = mContext.getResources().getDisplayMetrics();
        lPoint.x = metrics.widthPixels;
        lPoint.y = metrics.heightPixels;
        return lPoint;
    }
}
