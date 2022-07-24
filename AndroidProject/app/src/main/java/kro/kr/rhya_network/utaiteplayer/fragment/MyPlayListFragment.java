package kro.kr.rhya_network.utaiteplayer.fragment;

import android.content.ContentValues;
import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.fragment.app.Fragment;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;
import androidx.swiperefreshlayout.widget.SwipeRefreshLayout;

import org.json.JSONArray;
import org.json.JSONObject;

import java.lang.reflect.Array;
import java.net.URLDecoder;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.Iterator;
import java.util.Objects;

import kro.kr.rhya_network.utaiteplayer.R;
import kro.kr.rhya_network.utaiteplayer.activity.ActivityMain;
import kro.kr.rhya_network.utaiteplayer.adapter.RhyaMyPlayListAdapter;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaAsyncTask;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaHttpsConnection;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaPlayList;

public class MyPlayListFragment extends Fragment {
    // Play list
    public ArrayList<RhyaPlayList> rhyaPlayLists;
    // Auth token
    private String authToken = null;
    // Adapter
    private RhyaMyPlayListAdapter rhyaMyPlayListAdapter;
    // UI Object
    private kro.kr.rhya_network.utaiteplayer.utils.RhyaSwipeRefreshLayout swipeLayout;
    private TextView noItemTitle;
    // Is reload
    public boolean isReload = false;




    public MyPlayListFragment() {
        // Required empty public constructor
    }

    public static MyPlayListFragment newInstance() {
        return new MyPlayListFragment();
    }

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {

        // Inflate the layout for this fragment
        return inflater.inflate(R.layout.fragment_my_play_list, container, false);
    }

    @Override
    public void onViewCreated(@NonNull View view, @Nullable Bundle savedInstanceState) {
        super.onViewCreated(view, savedInstanceState);

        // UI Object
        swipeLayout = view.findViewById(R.id.swipeLayout);
        noItemTitle = view.findViewById(R.id.noItemTitle);


        // Set listener
        swipeLayout.setOnRefreshListener(this::changeDataValue);

        // UI Init
        RecyclerView recyclerView = view.findViewById(R.id.recyclerView);
        rhyaMyPlayListAdapter = new RhyaMyPlayListAdapter(requireActivity(), this);
        LinearLayoutManager linearLayoutManager = new LinearLayoutManager(getContext());
        linearLayoutManager.setOrientation(LinearLayoutManager.VERTICAL);
        recyclerView.setAdapter(rhyaMyPlayListAdapter);
        recyclerView.setLayoutManager(linearLayoutManager);

        // 변수 초기화
        if (rhyaPlayLists == null) {
            rhyaPlayLists = new ArrayList<>();
            changeDataValue();
        }else {
            if (isReload) {
                changeDataValue();
                isReload = false;
            }else {
                rhyaMyPlayListAdapter.setList(rhyaPlayLists);
            }
        }
    }


    @Override
    public void onResume() {
        super.onResume();

        if (rhyaPlayLists.size() > 1) {
            noItemTitle.setVisibility(View.GONE);
        }else {
            noItemTitle.setVisibility(View.VISIBLE);
        }

        try {
            if (rhyaPlayLists == null) {
                rhyaPlayLists = new ArrayList<>();
                changeDataValue();
            }else {
                if (isReload) {
                    changeDataValue();
                    isReload = false;
                }else {
                    rhyaMyPlayListAdapter.setList(rhyaPlayLists);
                }
            }
        }catch (Exception ignored) { }
    }

    public void changeDataValue() {
        loadData(result -> {
            swipeLayout.setRefreshing(false);

            if (rhyaPlayLists.size() > 1) {
                noItemTitle.setVisibility(View.GONE);
            }else {
                noItemTitle.setVisibility(View.VISIBLE);
            }

            switch (result) {

                case "error1": {
                    Toast.makeText(requireContext(), "서버 접속 중에 오류가 발생하였습니다. (00078)", Toast.LENGTH_SHORT).show();

                    break;
                }

                case "error2": {
                    Toast.makeText(requireContext(), "JSON 구문분석 중 오류가 발생하였습니다. (00089)", Toast.LENGTH_SHORT).show();

                    break;
                }

                case "fail": {
                    Toast.makeText(requireContext(), "서버가 접근을 명령을 거부하였습니다. (00080)", Toast.LENGTH_SHORT).show();

                    break;
                }

                case "success": {
                    rhyaMyPlayListAdapter.setList(rhyaPlayLists);

                    break;
                }
            }
        });
    }
    public interface LoadEndEvent {
        void loadEndNextTask(String result);
    }
    public void loadData(LoadEndEvent loadEndEvent) {
        ((ActivityMain) requireActivity()).showTaskDialog();

        // 데이터 로딩
        new RhyaAsyncTask<String, String>() {
            @Override
            protected void onPreExecute() {
                rhyaPlayLists.clear();
                rhyaPlayLists.add(null);
            }

            @Override
            protected String doInBackground(String arg) {
                try {
                    authToken = ((ActivityMain) requireActivity()).rhyaCore.getAutoLogin(((ActivityMain) requireActivity()).rhyaSharedPreferences, requireContext());

                    RhyaHttpsConnection rhyaHttpsConnection = new RhyaHttpsConnection();
                    ContentValues urlParam = new ContentValues();

                    urlParam.put("mode", "0");
                    urlParam.put("auth", authToken);
                    urlParam.put("name", ((ActivityMain) requireActivity()).rhyaCore.SERVICE_APP_NAME);

                    try {
                        JSONObject jsonObject = new JSONObject(rhyaHttpsConnection.request(((ActivityMain) requireActivity()).rhyaCore.MAIN_URL, urlParam));
                        String result = jsonObject.getString("result");
                        if (result.equals("success")) {
                            String playList = URLDecoder.decode(jsonObject.getString("play_list"), "UTF-8");

                            // 사용자 플레이 리스트
                            JSONObject playListJsonObject = new JSONObject(playList);
                            for (Iterator<String> iter = playListJsonObject.keys(); iter.hasNext(); ) {
                                String key = iter.next();
                                JSONArray jsonArray = new JSONArray(playListJsonObject.getJSONArray(key).toString());
                                RhyaPlayList rhyaPlayList = new RhyaPlayList();
                                ArrayList<String> tempArrayList = new ArrayList<>();

                                for (int i = 0; i < jsonArray.length(); i++) {
                                    String value = jsonArray.getString(i);
                                    if (value.contains("_IMAGE_TYPE_")) {
                                        rhyaPlayList.setImageType(Integer.parseInt(value.replaceAll("_IMAGE_TYPE_", "")));

                                        continue;
                                    }

                                    if (value.contains("_NAME_")) {
                                        rhyaPlayList.setName(value.replaceAll("_NAME_", ""));

                                        continue;
                                    }

                                    tempArrayList.add(jsonArray.getString(i));
                                }

                                rhyaPlayList.setUuid(key);
                                rhyaPlayList.setMusicList(tempArrayList);
                                rhyaPlayLists.add(rhyaPlayList);
                            }

                            return "success";
                        }else {
                            return "fail";
                        }
                    }catch (Exception ex) {
                        ex.printStackTrace();

                        return "error2";
                    }
                }catch (Exception ex) {
                    ex.printStackTrace();

                    return "error1";
                }
            }

            @Override
            protected void onPostExecute(String result) {
                ((ActivityMain) requireActivity()).dismissTaskDialog();
                if (loadEndEvent != null)
                    loadEndEvent.loadEndNextTask(result);
            }
        }.execute(null);
    }
}
