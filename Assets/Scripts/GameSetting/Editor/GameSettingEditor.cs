using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace UnityGame.Data
{
    public class GameSettingEditor : SettingBaseEditor
    {
        public override void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            if (m_Root == null)
            {
                m_Root = rootVisualElement;
                GameSetting gameSetting = m_Asset as GameSetting;
                SerializedObject so = new SerializedObject(gameSetting);
                // Import UXML
                var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/GameSetting/Editor/SettingBaseEditor.uxml");
                visualTree.CloneTree(m_Root);

                ScrollView scrollView = m_Root.Q<ScrollView>();

                #region Stamina
                Foldout staminaField = new FoldoutExtend("体力 スタミナ", CustomFoldOutClass);
                staminaField.Add(new SliderExtend("基礎体力", "System.StaminaBase", so, true, 0, 100));
                staminaField.Add(new SliderExtend("回復量(秒)", "System.StaminaIncrease", so, true, 0, 100));
                staminaField.Add(new ColorFieldExtend("フルーのキャラ色", "System.StaminaColorMax", so));
                staminaField.Add(new ColorFieldExtend("切れのキャラ色", "System.StaminaColorMin", so));
                staminaField.Add(new SliderExtend("減る量(毎回)", "System.StaminaReduce", so, true, 0, 100));
                #endregion

                #region Calories
                Foldout caloriesField = new FoldoutExtend("カロリー", CustomFoldOutClass);
                caloriesField.Add(new SliderExtend("基礎減少量", "System.CalorieReduceBase", so, true, 0, 100));
                #endregion

                #region Speed
                Foldout speedField = new FoldoutExtend("スピード", CustomFoldOutClass);
                speedField.Add(new SliderExtend("スピード増加(毎回)", "System.SpeedGangIncrease", so, true, 0, 100));
                speedField.Add(new SliderExtend("スピード減少(秒)", "System.SpeedGangReduce", so, true, 0, 100));
                speedField.Add(new MinMaxSliderField("縄飛ぶスピード", "System.JumpSpeed", so, 0.1f, 10));
                #endregion

                #region Sweat
                Foldout sweatField = new FoldoutExtend("汗", CustomFoldOutClass);
                sweatField.Add(new SliderIntExtend("汗の量", "System.SweatEachPoint", so, true, 0, 100));
                sweatField.Add(new PropertyFieldExtend("汗の数", "System.SweatCountRate", so));
                #endregion

                #region Coin
                Foldout coinField = new FoldoutExtend("コイン", CustomFoldOutClass);
                coinField.Add(new SliderIntExtend("基礎貰えるコイン", "System.JumpRopeCoinBase", so, true, 0, 100));
                coinField.Add(new SliderExtend("効果飛ぶの強さ", "System.CoinJumpForce", so, true, 0, 100));
                coinField.Add(new SliderExtend("効果飛ぶの時間", "System.CoinJumpTime", so, true, 0, 100));
                coinField.Add(new SliderExtend("効果飛ぶの大きさ", "System.CoinJumpFinalScale", so, 0, 100));
                //coinField.Add(new PropertyFieldExtend("効果飛ぶののEase", "System.CoinJumpEase", so));
                #endregion

                scrollView.Add(staminaField);
                scrollView.Add(caloriesField);
                scrollView.Add(speedField);
                scrollView.Add(sweatField);
                scrollView.Add(coinField);
            }

        }
    }
}