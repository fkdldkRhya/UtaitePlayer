package kro.kr.rhya_network.utaiteplayer.utils;

import android.content.Context;
import android.util.AttributeSet;
import android.view.MotionEvent;
import android.view.ViewConfiguration;

import androidx.swiperefreshlayout.widget.SwipeRefreshLayout;

public class RhyaSwipeRefreshLayout extends SwipeRefreshLayout {
    private final int mTouchSlop;

    private float mPrevX;



    public RhyaSwipeRefreshLayout(Context context, AttributeSet attrs) {

        super(context, attrs);



        mTouchSlop = ViewConfiguration.get(context).getScaledTouchSlop();

    }



    @Override

    public boolean onInterceptTouchEvent(MotionEvent event) {



        switch (event.getAction()) {

            case MotionEvent.ACTION_DOWN:

                mPrevX = MotionEvent.obtain(event).getX();

                break;



            case MotionEvent.ACTION_MOVE:

                final float eventX = event.getX();

                float xDiff = Math.abs(eventX - mPrevX);



                if (xDiff > mTouchSlop) {

                    return false;

                }

        }

        return super.onInterceptTouchEvent(event);
    }
}
