import 'dart:developer';
import 'package:cloud_firestore/cloud_firestore.dart';
import 'package:cloud_functions/cloud_functions.dart';
import 'package:firebase_auth/firebase_auth.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_svg/flutter_svg.dart';
import 'package:intl/intl.dart';
import 'package:nutri_track_ua/auth/sing_in_with_google.dart';
import 'package:nutri_track_ua/components/alert_dialog_custom.dart';
import 'package:nutri_track_ua/components/animation_route.dart';
import 'package:nutri_track_ua/database%20components/user_everyday_sesion_provider.dart';
import 'package:nutri_track_ua/view/user_auth_views/start_view.dart';

class MainMenuView extends ConsumerStatefulWidget {
  const MainMenuView({super.key});

  @override
  _MainMenuViewState createState() => _MainMenuViewState();
}

class _MainMenuViewState extends ConsumerState<MainMenuView> {
  late Future<void> _initializeData;

  @override
  void initState() {
    super.initState();
    final userId = FirebaseAuth.instance.currentUser?.uid;
    // Ініціалізація даних користувача при запуску
    _initializeData = _initializeUserData(userId);
  }

  /// Ініціалізує дані користувача, викликаючи Firebase Cloud Function
  Future<void> _initializeUserData(String? userId) async {
    if (userId != null) {
      try {
        final HttpsCallable callable = FirebaseFunctions.instance
            .httpsCallable('createOrUpdateDailyDocument');
        await callable.call({'userId': userId});
      } catch (e) {
        // Лог помилки у випадку невдалого виклику Cloud Function
        log('Failed to call Cloud Function: $e');
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    final userSession = ref.watch(userSessionProvider);
    final userId = FirebaseAuth.instance.currentUser?.uid;

    return Scaffold(
      appBar: AppBar(
        title: const Text(
          "NutriTrack",
          style: TextStyle(
            color: Color(0xFFE6E6E6),
            fontSize: 21,
          ),
        ),
        actions: [
          // Відображення кількості днів користування для поточного користувача
          userSession.when(
            data: (loginDays) => Row(
              children: [
                GestureDetector(
                  onTap: () {},
                  child: SvgPicture.asset(
                    'assets/images/fire.svg',
                    width: 40,
                    height: 40,
                    colorFilter: const ColorFilter.mode(
                      Color(0xFFE6E6E6),
                      BlendMode.srcIn,
                    ),
                  ),
                ),
                const SizedBox(width: 8),
                Text(
                  '$loginDays днів',
                  style: const TextStyle(
                      color: Color(0xFFE6E6E6),
                      fontSize: 16,
                      fontWeight: FontWeight.bold),
                ),
                const SizedBox(width: 16),
              ],
            ),
            loading: () => const Row(
              children: [
                CircularProgressIndicator(),
                SizedBox(width: 8),
                Text(
                  'Завантаження...',
                  style: TextStyle(
                    color: Color(0xFFE6E6E6),
                    fontSize: 16,
                    fontWeight: FontWeight.bold,
                  ),
                ),
              ],
            ),
            error: (error, stackTrace) => const Text(
              'Помилка завантаження днів',
              style: TextStyle(
                color: Color(0xFFE6E6E6),
                fontSize: 16,
                fontWeight: FontWeight.bold,
              ),
            ),
          ),
        ],
        backgroundColor: const Color(0xFF2F4F4F),
        leading: IconButton(
          icon: const Icon(
            Icons.logout_outlined,
            size: 36,
            color: Color(0xFFE6E6E6),
          ),
          onPressed: () {
            _handleSignOut(context); // Обробка виходу користувача з акаунту
          },
        ),
      ),
      body: FutureBuilder(
        future: _initializeData,
        builder: (context, snapshot) {
          // Обробка стану ініціалізації даних
          if (snapshot.connectionState == ConnectionState.waiting) {
            return const Center(child: CircularProgressIndicator());
          }
          if (snapshot.hasError) {
            return const Center(
              child: Text(
                'Помилка ініціалізації даних',
                style: TextStyle(
                  fontSize: 16,
                  color: Colors.white,
                ),
              ),
            );
          }
          return SingleChildScrollView(
            padding: const EdgeInsets.all(16.0),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const Text(
                  'Сьогодні',
                  style: TextStyle(
                    fontSize: 24,
                    fontWeight: FontWeight.bold,
                  ),
                ),
                const SizedBox(height: 20),
                Container(
                  padding: const EdgeInsets.all(16.0),
                  decoration: BoxDecoration(
                    color: const Color(0xFF82A4A6),
                    borderRadius: BorderRadius.circular(8.0),
                  ),
                  // Відображення даних про споживання та норми
                  child: FutureBuilder<Map<String, dynamic>>(
                    future: userId != null
                        ? _getDailyData(userId)
                        : Future.value({}),
                    builder: (context, snapshot) {
                      if (snapshot.connectionState == ConnectionState.waiting) {
                        return const CircularProgressIndicator();
                      }
                      if (snapshot.hasError) {
                        return const Text(
                          'Не вдалося завантажити дані',
                          style: TextStyle(
                            fontSize: 16,
                            color: Colors.white,
                          ),
                        );
                      }
                      final data = snapshot.data ?? {};
                      return Column(
                        mainAxisAlignment: MainAxisAlignment.center,
                        children: [
                          const Text(
                            'Необхідно:',
                            style: TextStyle(
                              fontSize: 16,
                              color: Colors.white,
                            ),
                          ),
                          Text(
                            '${data['dailyCalories'].toString()} кал',
                            style: const TextStyle(
                              fontSize: 42,
                              color: Colors.white,
                            ),
                          ),
                          const SizedBox(height: 10),
                          // Відображення макронутрієнтів
                          FutureBuilder<Map<String, dynamic>>(
                            future: userId != null
                                ? _getMacronutrients(userId)
                                : Future.value(
                                    {'carbs': 0, 'proteins': 0, 'fats': 0}),
                            builder: (context, macronutrientsSnapshot) {
                              if (macronutrientsSnapshot.connectionState ==
                                  ConnectionState.waiting) {
                                return const CircularProgressIndicator();
                              }
                              if (macronutrientsSnapshot.hasError) {
                                return const Text(
                                  'Не вдалося завантажити макронутрієнти',
                                  style: TextStyle(
                                    fontSize: 16,
                                    color: Colors.white,
                                  ),
                                );
                              }
                              final macronutrients =
                                  macronutrientsSnapshot.data ??
                                      {
                                        'carbs': 0,
                                        'proteins': 0,
                                        'fats': 0,
                                        'carbsConsumed': 0,
                                        'proteinsConsumed': 0,
                                        'fatsConsumed': 0,
                                      };
                              return Row(
                                mainAxisAlignment:
                                    MainAxisAlignment.spaceAround,
                                children: [
                                  // Побудова інформації про кожен макронутрієнт
                                  _buildNutrientInfo(
                                      'Вуглеводи:',
                                      macronutrients['carbsConsumed'] ?? 0,
                                      macronutrients['carbs'] ?? 0),
                                  _buildNutrientInfo(
                                      'Білок:',
                                      macronutrients['proteinsConsumed'] ?? 0,
                                      macronutrients['proteins'] ?? 0),
                                  _buildNutrientInfo(
                                      'Жири:',
                                      macronutrients['fatsConsumed'] ?? 0,
                                      macronutrients['fats'] ?? 0),
                                ],
                              );
                            },
                          ),
                        ],
                      );
                    },
                  ),
                ),
              ],
            ),
          );
        },
      ),
      bottomNavigationBar: BottomNavigationBar(
        backgroundColor: const Color(0xFF2F4F4F),
        items: const [
          BottomNavigationBarItem(
            icon: Icon(
              Icons.book,
              size: 37,
            ),
            label: 'Записник',
          ),
          BottomNavigationBarItem(
            icon: Icon(
              Icons.fitness_center,
              size: 37,
            ),
            label: 'Активності',
          ),
          BottomNavigationBarItem(
            icon: Icon(
              Icons.person,
              size: 37,
            ),
            label: 'Профіль',
          ),
        ],
        currentIndex: 0,
        onTap: (index) {
          // Обробка зміни вкладки (можливо, варто додати обробник для зміни сторінки)
        },
        selectedItemColor: const Color.fromARGB(255, 120, 231, 185),
        unselectedItemColor: const Color.fromARGB(255, 230, 230, 230),
      ),
    );
  }

  // Метод для відображення інформації про макронутрієнти
  Widget _buildNutrientInfo(String label, int consumed, int total) {
    return Column(
      children: [
        Text(
          label,
          style: const TextStyle(
            fontSize: 16,
            color: Colors.white,
          ),
        ),
        Text(
          '$consumed/$total',
          style: const TextStyle(
            fontSize: 16,
            color: Colors.white,
          ),
        ),
      ],
    );
  }

  // Метод для отримання даних за сьогоднішній день
  Future<Map<String, dynamic>> _getDailyData(String userId) async {
    try {
      final today = DateFormat('dd.MM.yyyy').format(DateTime.now());
      final doc = await FirebaseFirestore.instance
          .collection('users')
          .doc(userId)
          .collection('days')
          .doc(today)
          .get();
      return doc.exists ? doc.data() ?? {} : {};
    } catch (e) {
      log('Error fetching daily data: $e');
      return {};
    }
  }

  // Метод для отримання макронутрієнтів
  Future<Map<String, dynamic>> _getMacronutrients(String userId) async {
    try {
      final today = DateFormat('dd.MM.yyyy').format(DateTime.now());
      final doc = await FirebaseFirestore.instance
          .collection('users')
          .doc(userId)
          .collection('days')
          .doc(today)
          .get();
      if (doc.exists) {
        return {
          'carbs': doc['carbs'] ?? 0,
          'proteins': doc['proteins'] ?? 0,
          'fats': doc['fats'] ?? 0,
          'carbsConsumed': doc['carbsConsumed'] ?? 0,
          'proteinsConsumed': doc['proteinsConsumed'] ?? 0,
          'fatsConsumed': doc['fatsConsumed'] ?? 0,
        };
      }
      return {};
    } catch (e) {
      log('Error fetching macronutrients: $e');
      return {};
    }
  }

  void _handleSignOut(BuildContext context) async {
    bool? result = await CustomDialogAlert.showConfirmationDialog(
      context,
      'Вихід з аккаунту',
      'Ви впевнені, що хочете вийти з аккаунту?',
    );
    if (result != null && result) {
      bool isUserSignOut = await signOut();
      if (isUserSignOut) {
        Navigator.pushReplacement(
            context, CustomPageRoute(page: const StartView()));
      }
    }
  }
}
