package kro.kr.rhya_network.utaiteplayer.lib.com_github_amarjain07_StickyScrollView_1_0_2.provider;

import android.content.Context;
import android.content.res.TypedArray;
import androidx.annotation.StyleableRes;
import android.util.AttributeSet;

import kro.kr.rhya_network.utaiteplayer.lib.com_github_amarjain07_StickyScrollView_1_0_2.provider.interfaces.IResourceProvider;

/**
 * Created by Amar Jain on 17/03/17.
 */

public class ResourceProvider implements IResourceProvider {

    private final TypedArray mTypeArray;

    public ResourceProvider(Context context, AttributeSet attrs, @StyleableRes int[] styleRes) {
        mTypeArray = context.obtainStyledAttributes(attrs, styleRes);
    }

    @Override
    public int getResourceId(@StyleableRes int styleResId) {
        return mTypeArray.getResourceId(styleResId, 0);
    }

    @Override
    public void recycle() {
        mTypeArray.recycle();
    }
}
