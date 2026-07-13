using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Timeline.Scaffolding;

namespace Laughman.LaughmanCode.Timeline;

// 招笑人的时间线故事列。
// id 由 StoryKey 经 StringHelper.Slugify 派生（"laughman" → "LAUGHMAN"）。
// [RegisterStory] 触发 RitsuLib 把本 story 注册进 StoryModel——缺了它，
// 点击带 StoryId 的 epoch 时 StoryModel.Get("LAUGHMAN") 会抛异常并锁死输入。
// 纪元顺序由各 Epoch 上的 [RegisterStoryEpoch(typeof(LaughmanModStory))] 决定，无需手写 Epochs。
[RegisterStory]
public sealed class LaughmanModStory : ModStoryTemplate
{
    protected override string StoryKey => "laughman";
}
